Country Shapefiles from GBD Tool for Use in BenMAP-CE
(Sept. 29, 2015)


BenMAP_country_files.zip contains a folder for each country contained in the BenMAP-CE Global Burden of Disease Rollback Tool.  The files contained in these folders are for use in BenMAP-Community Edition (BenMAP-CE).  


The folders are named using the 3-character ISO code for each country.  The file "country_names.csv" contains a list of all countries and their ISO codes in alphabetical order.



Each folder contains the following for the specified country:


1) A 0.1 degree (approximately 10km) resolution shapefile grid (files ending in "_grid.shp")


2) A population dataset aligned with the shapefile grid (files ending in "_population.csv")

3) A baseline pollutant concentration value (in micrograms per cubic meter) for each grid cell (files ending in "_baseline_PM25.csv")




Below are simple instructions for using the GBD Country shapefiles in BenMAP-CE. For more information on how to work with these files and the sources of these data, please refer to the BenMAP-CE Quick Start Guide or the BenMAP-CE User's Manual.


Add a Shapefile Grid:

To add a country's grid, click on MODIFY DATASETS from the BenMAP-CE Main Menu.  Under GRID DEFINITIONS add a SHAPEFILE GRID.  The "Country Shapefiles" folder is located under "My BenMAP-CE Files." Select the country's shapefile and complete the load process.



Add a Population Dataset:

Population datasets are also added using MODIFY DATASETS, under the POPULATION DATASETS heading.






Add Pollutant Concentrations:

First, double-click POLLUTANT in the BenMAP-CE tree menu and specify "PM2.5". Then, Double-click BASELINE under SOURCE OF AIR QUALITY DATA in the tree menu. In the window that opens, ensure your grid type is set to the country grid you imported above. Select MODEL DATA and click NEXT. Click the OPEN FOLDER icon and navigate to the "_baseline_PM25.csv" file for the country you have selected above. Click OPEN. Then click VALIDATE to ensure the data formats are correct. Finally, Click OK, enter a name for the AQGX file and click SAVE.

Now double-click "_baseline_PM25.csv" in the BenMAP-CE tree menu. This will display a map of your baseline PM2.5 concentrations over your country's grid.



Because BenMAP-CE currently only supports "rollback" of monitoring data, not modeled data, we have included a simple Python program to assist in creating a CONTROL layer.  The "benmap_baseline_rollback_tool" is provided in the "Country Files" folder.  To use this tool, double-click on the benmap_baseline_rollback_tool.exe file. The program will guide you through the steps of selecting a "_baseline_PM25.csv" file and specifying a percentage to rollback the pollutant concentrations.  Once you have created the "_control_x_percent_rollback_PM25.csv" file, you can load it as the CONTROL layer in BenMAP-CE similar to the BASELINE layer described above.
