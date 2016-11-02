// -----------------------------------------------------------------------
// <copyright file="ClipRaster.cs" company="DotSpatial Team">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using DotSpatial.Data;
using DotSpatial.Analysis;
using BenMAP;
using System.IO;
using System.Diagnostics;
using DotSpatial.NTSExtension;

namespace benmap
{
    /// <summary>
    /// Clip input raster with polygon.
    /// </summary>
    public static class ClipRasterRTI
    {
        /// <summary>
        /// Finds the X-coordinate of the first scanline
        /// </summary>
        /// <param name="polygon">The polygon</param>
        /// <param name="inputRaster">The input raster</param>
        /// <returns>the X-coordinate of the first scanline</returns>
        private static double GetXStart(IFeature polygon, IRaster inputRaster)
        {
            double rasterMinXCenter = inputRaster.Xllcenter;

            // Does the poly sit to the left of the raster or does the raster start before the left edge of the poly
            if (polygon.Geometry.EnvelopeInternal.Minimum.X < rasterMinXCenter)
                return rasterMinXCenter;
            else
                return FirstColumnToProcess(polygon.Geometry.EnvelopeInternal.Minimum.X, rasterMinXCenter, inputRaster.CellWidth);
        }

        /// <summary>
        /// Finds the first raster column corresponding to the left-most edge of the poly
        /// </summary>
        /// <param name="polygon">the polygon</param>
        /// <param name="inputRaster">the input raster</param>
        /// <returns>The raster column corresponding to the left-most edge of the poly 
        /// (if raster starts before left edge of the poly)
        /// or the first raster column (if raster starts after left edge of the poly)</returns>
        /// <remarks>If the poly sits to the left of the raster then the first column of the raster is returned.</remarks>
        private static int GetStartColumn(IFeature polygon, IRaster inputRaster)
        {
            double rasterMinXCenter = inputRaster.Xllcenter;

            // Does the poly sit to the left of the raster or does the raster start before the left edge of the poly
            if (polygon.Geometry.EnvelopeInternal.Minimum.X < rasterMinXCenter)
                return 0;
            else
                return ColumnIndexToProcess(polygon.Geometry.EnvelopeInternal.Minimum.X, rasterMinXCenter, inputRaster.CellWidth);
        }

        /// <summary>
        /// Finds the last raster column corresponding to the right-most edge of the poly
        /// </summary>
        /// <param name="polygon">the polygon</param>
        /// <param name="inputRaster">the input raster</param>
        /// <returns>The raster column corresponding to the right-most edge of the poly 
        /// (if raster ends after the right edge of the poly)
        /// or the last raster column (if raster ends before right edge of the poly)</returns>
        private static int GetEndColumn(IFeature polygon, IRaster inputRaster)
        {
            double rasterMaxXCenter = inputRaster.Extent.MaxX - inputRaster.CellWidth / 2;

            // Does the poly sit to the right of the raster or does the raster end after the right edge of the poly
            if (polygon.Geometry.EnvelopeInternal.Right() > rasterMaxXCenter)
                return inputRaster.NumColumns - 1;
            else
                return ColumnIndexToProcess(polygon.Geometry.EnvelopeInternal.Right(), rasterMaxXCenter, inputRaster.CellWidth);
        }

        /// <summary>
        /// Clips a raster with a polygon feature
        /// </summary>
        /// <param name="inputFileName">The input raster file</param>
        /// <param name="polygon">The clipping polygon feature</param>
        /// <param name="outputFileName">The output raster file</param>
        /// <param name="cancelProgressHandler">Progress handler for reporting progress status and cancelling the operation</param>
        /// <returns>The output clipped raster object</returns>
        public static double ClipRasterWithPolygon(string inputFileName, IFeature polygon, string outputFileName,
                                                    ICancelProgressHandler cancelProgressHandler = null)
        {
            return ClipRasterWithPolygon(polygon, Raster.Open(inputFileName), outputFileName, false, cancelProgressHandler);
        }

        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("HH:mm:ss.ffff");
        }


        /// <summary>
        /// Clips a raster with a polygon feature
        /// </summary>
        /// <param name="polygon">The clipping polygon feature</param>
        /// <param name="input">The input raster object</param>
        /// <param name="outputFileName">the output raster file name</param>
        /// <param name="cancelProgressHandler">Progress handler for reporting progress status and cancelling the operation</param>
        /// <remarks>We assume there is only one part in the polygon. 
        /// Traverses the raster with a vertical scan line from left to right, bottom to top</remarks>
        /// <returns></returns>
        public static double ClipRasterWithPolygon(IFeature polygon, IRaster input, string outputFileName,Boolean doSingleColumnClips,
                                                    ICancelProgressHandler cancelProgressHandler = null)
        {
            double sum = 0.0;
            String tempRasterDir= CommonClass.DataFilePath + @"\Tmp";
            //if the polygon is completely outside the raster
            if (input == null || polygon == null)
            {
                Console.WriteLine("Got a null");
            }
            if (!input.ContainsFeature(polygon))
                return 0.0;

            if (cancelProgressHandler != null)
                cancelProgressHandler.Progress(null, 0, "Retrieving the borders.");
            //Console.WriteLine(GetTimestamp(DateTime.Now)+": Getting borders");
            List<Border> borders = GetBorders(polygon);
            //Console.WriteLine(GetTimestamp(DateTime.Now) + ": Done getting borders");

            if (cancelProgressHandler != null)
                cancelProgressHandler.Progress(null, 0, "Copying raster.");

            //create output raster
            //IRaster output = Raster.CreateRaster(outputFileName, input.DriverCode, input.NumColumns, input.NumRows, 1,
            //                                     input.DataType, new[] { string.Empty });
            //output.Bounds = input.Bounds.Copy();
            //output.NoDataValue = input.NoDataValue;
            //if (input.CanReproject)
            //{
            //    output.Projection = input.Projection;
            //}

            // set all initial values of Output to NoData
            //for (int i = 0; i < output.NumRows; i++)
            //{
            //    for (int j = 0; j < output.NumColumns; j++)
            //    {
            //        output.Value[i, j] = output.NoDataValue;
            //    }
            //}

            double xStart = GetXStart(polygon, input);
            int columnStart = GetStartColumn(polygon, input); //get the index of first column
            double xCurrent = xStart;
            //Console.WriteLine(GetTimestamp(DateTime.Now) + ": xStart: " + xStart + ", colStart: " + columnStart + ", xCurrent: ");
            ProgressMeter pm = new ProgressMeter(cancelProgressHandler, "Clipping Raster", input.NumColumns);
            pm.StepPercent = 5;
            pm.StartValue = columnStart;

            int col = 0;
            int colToUse = 0;
            IRaster rasterToUse = null;
            String tempRasterFullPath = null;
            long start = 0;
            //Stopwatch st = new Stopwatch();
            //CommonClass.Debug = true;
            //Console.WriteLine("Starting at " + columnStart + ", ending at " + input.NumColumns);
            for (int columnCurrent = columnStart; columnCurrent < input.NumColumns; columnCurrent++)
            {
                //st.Start();
                xCurrent = xStart + col * input.CellWidth;
                start = DateTime.Now.Ticks;
                //Console.WriteLine(GetTimestamp(DateTime.Now) + ": starting col "+columnCurrent);
                //check sizze
                FileInfo f = new FileInfo(input.Filename);
                //Console.WriteLine("Got file of size : " + f.Length);
                colToUse = columnCurrent;
                rasterToUse = input;
                
                tempRasterFullPath = null;
                if (doSingleColumnClips)
                {
                    //chop it a little more
                    double minx = xCurrent-.5*input.CellWidth;
                    double maxy = input.Bounds.Top();
                    double maxx = xCurrent + .5 * input.CellWidth;
                    double miny= input.Bounds.Bottom();

                    //Console.WriteLine("minx: " + minx + ", miny: " + miny + ", maxx: " + maxx + ", maxy: " + maxy);

                    ProcessStartInfo warpStep = new System.Diagnostics.ProcessStartInfo();
                    string gdalWarpEXELoc = (new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location)).Directory.ToString();
                    gdalWarpEXELoc += @"\GDAL-EXE\gdalwarp.exe";
                    warpStep.FileName = gdalWarpEXELoc;
                    tempRasterFullPath = Path.Combine(tempRasterDir, "clippedRaster-SingleRow-" + Guid.NewGuid().ToString() + ".tif");
                    warpStep.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    warpStep.UseShellExecute = false;
                    warpStep.CreateNoWindow = true;
                    //warpStep.Arguments = "-ot Float32 ";
                    warpStep.Arguments += " -te " + minx + " " + miny + " " + maxx + " " + maxy + " ";
                    //warpStep.Arguments += "-t_srs \"+proj=merc +lon_0=0 +k=1 +x_0=0 +y_0=0 +a=6378137 +b=6378137 +units=m +no_defs \" ";
                    warpStep.Arguments += "\"" + input.Filename + "\" \"" + tempRasterFullPath + "\"";
                    try
                    {
                        // Start the process with the info we specified.
                        // Call WaitForExit and then the using statement will close.
                        using (Process exeProcess = Process.Start(warpStep))
                        {
                            exeProcess.WaitForExit();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Couldn't output: " + e.ToString());
                    }
                    colToUse = 0;
                    rasterToUse = Raster.Open(tempRasterFullPath);
                }

                var intersections = GetYIntersections(borders, xCurrent);
                //for our application we know that if the first and last entry
                //are same we need to pull out one of them to ensure
                //groups are picked correctly.
                //only matters if count >2
                if (CommonClass.Debug)
                {
                    Console.WriteLine("Raw from function");
                    for (int i = 0; i < intersections.Count; i++) // Loop with for.
                    {
                        Console.Write(intersections[i] + ",");
                    }
                    Console.WriteLine();
                    if (intersections.Count % 2 == 1)
                    {
                        Console.WriteLine("Diff on last record="+(intersections[0]-intersections[intersections.Count - 1]));
                    }

                }
                if (intersections.Count > 2 && intersections.Count%2==1)
                {
                    //if (intersections[0] == intersections[intersections.Count - 1])
                    //{
                        intersections.RemoveAt(intersections.Count - 1);
                    //}
                }
                if (CommonClass.Debug)
                {
                    Console.WriteLine("before sort");
                    for (int i = 0; i < intersections.Count; i++) // Loop with for.
                    {
                        Console.Write(intersections[i] + ",");
                    }
                    Console.WriteLine();

                }
                //Console.WriteLine(GetTimestamp(DateTime.Now) + ": Intesections done");
                intersections.Sort();
                if(CommonClass.Debug){
                    Console.WriteLine("After sort");
                    for (int i = 0; i < intersections.Count; i++) // Loop with for.
                    {
                        Console.Write(intersections[i]+",");
                    }
                    Console.WriteLine();
                }
                //Console.WriteLine(GetTimestamp(DateTime.Now) + ": Done sorting");
                sum += ParseIntersections(intersections, xCurrent, colToUse, rasterToUse);
                if (doSingleColumnClips)
                {
                    rasterToUse.Close();
                    Boolean deleted = false;
                    int counter = 5;
                    while (!deleted && counter > 0)
                    {
                        try
                        {
                            if (File.Exists(tempRasterFullPath) && tempRasterFullPath.ToUpper().Contains("TMP"))
                            {
                                File.Delete(tempRasterFullPath);
                            }

                            if (File.Exists(tempRasterFullPath + ".aux.xml") && tempRasterFullPath.ToUpper().Contains("TMP"))
                            {
                                File.Delete(tempRasterFullPath + ".aux.xml");
                            }
                            deleted = true;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Could not delete temp raster, waiting 1 second and trying again, " + counter + " attemps left.");
                            System.Threading.Thread.Sleep(1000);
                            counter--;
                        }
                    }
                }
                
                //Console.WriteLine(GetTimestamp(DateTime.Now) + ": Done parsing");
                
                // update progess meter
                pm.CurrentValue = xCurrent;
                //if (xCurrent % 20 == 0)
               // {
               //     Console.WriteLine("XCurrent " + xCurrent);
                //}
                //update counter
                col++;

                // cancel if requested
                if (cancelProgressHandler != null && cancelProgressHandler.Cancel)
                    return 0.0;
                //st.Stop();
                //Console.WriteLine("Seconds: " + st.Elapsed);            
                //st.Reset();
            }

            //output.Save();

            return sum;
        }

        /// <summary>
        /// Parses the intersections. Moves bottom to top.
        /// </summary>
        /// <param name="intersections">The intersections.</param>
        /// <param name="xCurrent">The x current.</param>
        /// <param name="column">The column.</param>
        /// <param name="output">The output.</param>
        /// <param name="input">The input.</param>
        private static double ParseIntersections(List<double> intersections, double xCurrent, int column,
                                               IRaster input)
        {
            //IRaster whatAmIDoing = (Raster)input.Clone();
            IRaster whatAmIDoing=null;
            if(CommonClass.Debug){
                if (!File.Exists(input.Filename + "-copy.tif"))
                {

                    File.Copy(input.Filename, input.Filename + "-copy.tif");
                }
                whatAmIDoing = Raster.Open(input.Filename + "-copy.tif");
                for (int x = 0; x < whatAmIDoing.NumRows; x++)
                {
                    for (int y = 0; y < whatAmIDoing.NumColumns; y++)
                    {
                        whatAmIDoing.Value[x, y] = 0;
                    }
                }
            }
            double sum = 0.0;
            double yStart = 0;
            double yEnd;
            bool nextIntersectionIsEndPoint = false;
            for (int i = 0; i < intersections.Count; i++)
            {
                if (!nextIntersectionIsEndPoint)
                {
                    // should be the bottom-most intersection
                    yStart = intersections[i];
                    nextIntersectionIsEndPoint = true;
                }
                else
                {
                    // should be the intersection just above the bottommost one.
                    yEnd = intersections[i];

                    int rowCurrent = input.NumRows - RowIndexToProcess(yStart, input.Extent.MinY, input.CellHeight);
                    int rowEnd = rowCurrent - (int)(Math.Ceiling((yEnd - yStart) / input.CellHeight));

                    //chop out just the rows we need

                    //traverse from bottom to top between the two intersections
                    if (CommonClass.Debug)
                    {
                        Console.WriteLine("Going from current " + rowCurrent + " to row end " + rowEnd + " for column " + column);
                    }

                    while (rowCurrent > rowEnd)
                    {
                        if (rowCurrent < 0 && rowEnd < 0) break;
                         if (CommonClass.Debug)
                        {
                            whatAmIDoing.Value[rowCurrent, column] = 1;
                         }
                        if (rowCurrent >= 0 && rowCurrent < input.NumRows)
                        {
                           sum += input.Value[rowCurrent, column];
                        }
                        if (CommonClass.Debug)
                        {
                            if (input.Value[rowCurrent, column] > 0)
                            {
                                Console.WriteLine("Got a value for " + rowCurrent + "," + column + ", of " + input.Value[rowCurrent, column] + " sum of " + sum);
                            }
                        }
                        rowCurrent--;
                    }
                    nextIntersectionIsEndPoint = false;
                }
            }

            if (CommonClass.Debug)
            {
                whatAmIDoing.SaveAs(@"p:\temp\curSet-" + column + "-"+xCurrent+"-"+Guid.NewGuid().ToString()+".tif");
                whatAmIDoing.Close();
            }
            

            return sum;
        }

        /// <summary>
        /// Gets the Y intersections.
        /// </summary>
        /// <param name="borders">The borders.</param>
        /// <param name="x">The line-scan x-value.</param>
        /// <returns></returns>
        private static List<double> GetYIntersections(IEnumerable<Border> borders, double x)
        {
            var intersections = new List<double>();

            foreach (Border border in borders)
            {
                var x1 = border.X1;
                var x2 = border.X2;

                // determine if the point lies inside of the border
                if (((x >= x1) && (x < x2)) || ((x >= x2) && (x < x1)))
                {
                    intersections.Add(border.M * x + border.Q);
                }
            }
            
            return intersections;
        }

        /// <summary>
        /// Gets the borders of the specified feature except vertical lines.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns></returns>
        private static List<Border> GetBorders(IFeature feature)
        {
            List<Border> borders = new List<Border>();
            
            //ShapeRange sr = feature.ParentFeatureSet.ShapeIndices[feature.ShapeIndex];
            ShapeRange sr = feature.ShapeIndex;
            Vertex lastVr = new Vertex() ;
            bool firstVRset = false;
            if (sr == null || sr.Parts == null)
            {
                Console.WriteLine("Sr or sr.parts is null!");
                return borders;
            }
            foreach (PartRange part in sr.Parts)
            {
                firstVRset = false;
                foreach (Vertex vert in part)
                {
                    if (!firstVRset)
                    {
                        lastVr = vert;
                        firstVRset = true;
                    }
                    else
                    {
                        Border border = new Border();
                        border.X1 = lastVr.X;
                        border.X2 = vert.X;


                        double y1 = lastVr.Y;
                        double y2 = vert.Y;
                        border.M = (y2 - y1) / (border.X2 - border.X1);
                        border.Q = y1 - (border.M * border.X1);

                        // if a line is a vertical line, it should not be added to the list of borders.
                        if (border.X1 != border.X2)
                            borders.Add(border);
                    }
                    lastVr = vert;
                }
            }
                

                /*
            for (int i = 0; i < feature.Coordinates.Count - 1; i++)
            {
                
                Border border = new Border();
                border.X1 = feature.Coordinates[i].X;
                border.X2 = feature.Coordinates[i + 1].X;

                double y1 = feature.Coordinates[i].Y;
                double y2 = feature.Coordinates[i + 1].Y;
                border.M = (y2 - y1) / (border.X2 - border.X1);
                border.Q = y1 - (border.M * border.X1);

                // if a line is a vertical line, it should not be added to the list of borders.
                if (border.X1 != border.X2)
                    borders.Add(border);
            }*/
            return borders;
        }

        /// <summary>
        /// Finds the x-coordinate of the first raster column to process
        /// </summary>
        /// <param name="xMinPolygon">The lowest left coordinate of the polygon.</param>
        /// <param name="xMinRaster">The lowest left coordinate of the raster.</param>
        /// <param name="cellWidth">Size of the cell.</param>
        private static double FirstColumnToProcess(double xMinPolygon, double xMinRaster, double cellWidth)
        {
            double columnIndex = Math.Ceiling((xMinPolygon - xMinRaster) / cellWidth);
            return xMinRaster + (columnIndex * cellWidth);
        }

        /// <summary>
        /// Finds the index of the first raster column to process
        /// </summary>
        /// <param name="xMinPolygon">The lowest left coordinate of the polygon.</param>
        /// <param name="xMinRaster">The lowest left coordinate of the raster.</param>
        /// <param name="cellWidth">Size of the cell.</param>
        private static int ColumnIndexToProcess(double xMinPolygon, double xMinRaster, double cellWidth)
        {
            return (int)Math.Ceiling((xMinPolygon - xMinRaster) / cellWidth);
        }

        /// <summary>
        /// Finds the index of the first raster row to process
        /// </summary>
        /// <param name="yMinPolygon">The lowest left coordinate of the polygon.</param>
        /// <param name="yMinRaster">The lowest left coordinate of the raster.</param>
        /// <param name="cellHeight">Size of the cell.</param>
        private static int RowIndexToProcess(double yMinPolygon, double yMinRaster, double cellHeight)
        {
            return (int)Math.Ceiling((yMinPolygon - yMinRaster) / cellHeight);
        }

        /// <summary>
        /// Finds the y-coordinate of the first raster row to process
        /// </summary>
        /// <param name="yMinPolygon">The lowest left coordinate of the polygon.</param>
        /// <param name="yMinRaster">The lowest left coordinate of the raster.</param>
        /// <param name="cellHeight">Size of the cell.</param>
        private static double FirstRowToProcess(double yMinPolygon, double yMinRaster, double cellHeight)
        {
            double rowIndex = Math.Ceiling((yMinPolygon - yMinRaster) / cellHeight);
            return yMinRaster + (rowIndex * cellHeight);
        }
    }
}
