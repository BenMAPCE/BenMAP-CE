using System;
using System.Data;

namespace BenMAP
{
    partial class IncidenceDatasetDefinition : FormBase
    {
        #region 私有变量或者属性

        private int _pageSize = 0;     //每页显示行数
        private int _totalRows = 0;         //总记录数
        private int _pageCount = 0;    //页数＝总记录数/每页显示行数
        private int _pageCurrent = 0;   //当前页号
        private int _currentRow = 0;      //当前记录行
        DataTable _dtColRowValue = new DataTable();        // 数据源

        #endregion 私有变量或者属性

        private void InitDataSet()
        {
            try
            {
                _pageSize = 25;      //设置页面行数
                _totalRows = _dtColRowValue.Rows.Count;
                _pageCount = (_totalRows / _pageSize);    //计算出总页数
                if ((_totalRows % _pageSize) > 0) { _pageCount++; }
                _pageCurrent = 1;    //当前页数从1开始
                _currentRow = 0;       //当前记录数从0开始
                LoadData();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void LoadData()
        {
            try
            {
                int startRow = 0;   //当前页面开始行
                int endRow = 0;     //当前页面结束行
                DataTable dtTemp = _dtColRowValue.Clone();   //克隆DataTable结构框架
                if (_pageCurrent == _pageCount)
                { endRow = _totalRows; }
                else
                { endRow = _pageSize * _pageCurrent; }
                startRow = _currentRow;
                //从元数据源复制记录行
                for (int i = startRow; i < endRow; i++)
                {
                    dtTemp.ImportRow(_dtColRowValue.Rows[i]);
                    _currentRow++;
                }

                txtCurrentPage.Enabled = true;      // 当前页
                txtCurrentPage.Text = _pageCurrent.ToString();
                lblPageCount.Text = string.Format("/{0}", _pageCount);      // 总页数
                //dgvInfo.DataSource = dtTemp;
                olvValues.DataSource = dtTemp;

                if (_pageCurrent == 1)
                {
                    tsbFirst.Enabled = false;
                    tsbPrevious.Enabled = false;
                    tsbNext.Enabled = true;
                    tsbLast.Enabled = true;
                }
                else if (_pageCurrent == _pageCount)
                {
                    tsbFirst.Enabled = true;
                    tsbPrevious.Enabled = true;
                    tsbNext.Enabled = false;
                    tsbLast.Enabled = false;
                }
                else if (_pageCurrent > 1 && _pageCurrent < _pageCount)
                {
                    tsbFirst.Enabled = true;
                    tsbPrevious.Enabled = true;
                    tsbNext.Enabled = true;
                    tsbLast.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
    }
}