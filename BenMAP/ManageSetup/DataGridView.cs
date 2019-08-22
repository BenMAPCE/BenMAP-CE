using System;
using System.Data;

namespace BenMAP
{
    partial class IncidenceDatasetDefinition : FormBase
    {

        private int _pageSize = 0; private int _totalRows = 0; private int _pageCount = 0; private int _pageCurrent = 0; private int _currentRow = 0; DataTable _dtColRowValue = new DataTable();

        private void InitDataSet()
        {
            try
            {
                _pageSize = 25; _totalRows = _dtColRowValue.Rows.Count;
                
                //_pageSize = _dtColRowValue.Rows.Count; _totalRows = _dtColRowValue.Rows.Count;//no need to paginate
                _pageCount = (_totalRows / _pageSize); if ((_totalRows % _pageSize) > 0) { _pageCount++; }
                _pageCurrent = 1; _currentRow = 0; LoadData();
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
                int startRow = 0; int endRow = 0; DataTable dtTemp = _dtColRowValue.Clone(); if (_pageCurrent == _pageCount)
                { endRow = _totalRows; }
                else
                { endRow = _pageSize * _pageCurrent; }
                startRow = _currentRow;
                for (int i = startRow; i < endRow; i++)
                {
                    dtTemp.ImportRow(_dtColRowValue.Rows[i]);
                    _currentRow++;
                }

                txtCurrentPage.Enabled = true; txtCurrentPage.Text = _pageCurrent.ToString();
                lblPageCount.Text = string.Format("/{0}", _pageCount); olvValues.DataSource = dtTemp;

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