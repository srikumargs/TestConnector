using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;

namespace SageConnectorConfiguration.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Returns the user-friendly name of this object.
        /// Child classes can set this property to a new value,
        /// or override it to determine the value on-demand.
        /// </summary>
        public virtual string DisplayName { get; protected set; }

        /// <summary>
        /// Raises the <see cref="E:PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The property name of the property that has changed.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            CheckPropertyName(propertyName);
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raises the <see cref="E:PropertyChanged"/> event.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyExpression"></param>
        protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            String propertyName = ExpressionHelper.MemberAccessAsString<T>(propertyExpression);
            RaisePropertyChanged(propertyName);
        }

        private void CheckPropertyName(string propertyName)
        {
            PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(this)[propertyName];
            if (propertyDescriptor == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                    "The property with the propertyName '{0}' doesn't exist.", propertyName));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate(object sender, PropertyChangedEventArgs e) { };

    }
}
