using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.ReinforceLab.iPhone.Controls.Calendar
{
    public class ViewCache
    {
        #region Variables
        readonly Dictionary<String, Stack<CacheableView>> _viewCache;
        readonly IViewFactory _factory;
        #endregion

        #region Constructor
        public ViewCache(IViewFactory factory)
        {
            _factory = factory;
            _viewCache = new Dictionary<string, Stack<CacheableView>>();    
        }
        #endregion

        #region Public methods
        public CacheableView GetView(string cell_id)
        {
            if(String.IsNullOrEmpty(cell_id)) return null;

            if (!_viewCache.ContainsKey(cell_id) || _viewCache[cell_id].Count == 0)
            {
                var view       = _factory.Create(cell_id);
                view.CellID    = cell_id;
                view.ViewCache = this;
                return view;
            }
            else
                return _viewCache[cell_id].Pop();
        }

        public void Enqueue(CacheableView view)
        {
            if (null == view || String.IsNullOrEmpty(view.CellID)) return;

            if (!_viewCache.ContainsKey(view.CellID))
                _viewCache[view.CellID] = new Stack<CacheableView>();
            _viewCache[view.CellID].Push(view);
        }
        #endregion
    }
}
