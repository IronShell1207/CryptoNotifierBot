using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Markup;
using System.Xaml;


namespace CryptoWidget.ViewModels.Base
{

    public abstract class ViewModel : MarkupExtension, INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Передача свойства и события для генерации события
        /// </summary>
        /// <param name="PropertyName"></param>
        protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
        /// <summary>
        /// Обновление значение свойства, для которого определено поле, в котором это свойство хранит свои данные. Также данный метод разрешает кольцевые изменения свойств. Когда система изменяет одно свойство, она может сразу обновлять и последующие
        /// </summary>
        /// <typeparam name="T">Ссылка на поле свойства</typeparam>
        /// <param name="field">Новое значение для установки в поле</param>
        /// <param name="value"></param>
        /// <param name="PropertyName">Автоматический параметр для компилятора (имя обновляемого свойства)</param>
        /// <returns></returns>
        protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string PropertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(PropertyName);
            return true;
        }

        /// <summary>
        /// Позволяет использовать ViewModel в фигурных скобках? внутри разметки и будет вызываться метод ProvideValue
        /// Который возвращает некий результат, который используется внутри разметки. Позволяет получить доступ к окну
        /// и свойству к которому привязывается ViewModel. 
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider sp)
        {
            var valueTargetService = sp.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            var rootObjectService = sp.GetService(typeof(IRootObjectProvider)) as IRootObjectProvider;

            OnInitialized(
                valueTargetService?.TargetObject,
                valueTargetService?.TargetProperty,
                rootObjectService?.RootObject);
            return this;
        }

        private WeakReference _TargetRef;
        private WeakReference _RootRef;

        public object TargetObject => _TargetRef.Target;

        public object RootObject
        {
            get => _RootRef.Target;
            set => _RootRef = new WeakReference(value);
        }

        protected virtual void OnInitialized(object Target, object Property, object Root)
        {
            _TargetRef = new WeakReference(Target);
            _RootRef = new WeakReference(Root);
        }


        private bool _Disposed;
        protected virtual void Dispose(bool Disposing)
        {
            if (!Disposing || _Disposed) return;
            _Disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}