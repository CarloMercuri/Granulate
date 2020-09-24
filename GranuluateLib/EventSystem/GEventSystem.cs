using System;
using System.Collections.Generic;
using System.Text;

namespace GranulateLibrary.EventSystem
{
    public class GEventSystem
    {
        public event EventHandler<PixelsModificationEventArgs> PixelsModification;

        protected virtual void OnPixelsModified(PixelsModificationEventArgs p)
        {
            EventHandler<PixelsModificationEventArgs> handler = PixelsModification;
            if (handler != null)
            {
                handler?.Invoke(this, p);
            }
        }

       
    }
}
