namespace WellCompareDir.WPF.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;

    /// <summary>
    ///     Parts from http://msdn.microsoft.com/en-us/magazine/dd419663.aspx.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        protected virtual bool ThrowOnInvalidPropertyName { get; set; }

        protected abstract void HandlePropertyChange(string propertyName);

        #region The usual INPC implementation

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }

            this.HandlePropertyChange(propertyName);
        }

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,  
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;

                if (this.ThrowOnInvalidPropertyName)
                {
                    throw new Exception(msg);
                }
                else
                {
                    Debug.Fail(msg);
                }
            }
        }

        #endregion
    }
}
