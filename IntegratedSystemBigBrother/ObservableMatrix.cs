using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;

namespace IntegratedSystemBigBrother
{
    public class ObservableMatrix<T> : 
            IEquatable<ObservableMatrix<T>>,
            IEnumerable, IEnumerable<T>, 
            INotifyCollectionChanged, INotifyPropertyChanged
        where T : IEquatable<T>
    {
        private T[,] _content;
        /// <summary>
        /// Контент матрицы.
        /// </summary>
        public T[,] Content
        {
            get { return _content; }
            set
            {
                 _content = value;
                OnPropertyChanged(nameof(Content));
            }
        }

        /// <summary>
        /// Константа названия свойства индексатора.
        /// </summary>
        private const string IndexerName = "Item[]";

        /// <summary>
        /// Индексатор, дающий доступ к значениям словаря по ключу.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public T this[int i, int j]
        {
            get { return _content[i, j]; }
            set
            {
                if (!Content[i, j]?.Equals(value) ?? true)
                {
                    Content[i, j] = value;
                    OnPropertyChanged(IndexerName);
                }
            }
        }

        /// <summary>
        /// Размер матрицы.
        /// </summary>
        public (int Rows, int Colums) Size
        {
            get { return (Content.GetUpperBound(0) + 1, Content.GetUpperBound(1) + 1); }
        }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="size1"></param>
        /// <param name="size2"></param>
        public ObservableMatrix(int size1, int size2) : base()
        {
            _content = new T[size1, size2];
        }

        /// <summary>
        /// Копирование контента матрицы в другую матрицу.
        /// </summary>
        /// <param name="receiver"></param>
        public void CopyTo(T[,] receiver)
        {
            this.Content.CopyTo(receiver, 0);
        }

        /// <summary>
        /// Событие уведомления подписчиков об изменении свойства.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Метод вызова события уведомления подписчиков об изменении свойства.
        /// </summary>
        /// <param name="prop"></param>
        private void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        /// <summary>
        /// Событие уведомленияы подписчиков на событие изменения коллекции.
        /// </summary>    
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Реализация интерфейса перечислимого объекта.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return (IEnumerator<T>)_content.GetEnumerator();
        }

        /// <summary>
        /// Реализация интерфейса перечислимого объекта.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Сравнение с другой наблюдаемой матрицей.
        /// </summary>
        /// <param name="second"></param>
        /// <returns></returns>
        public bool Equals(ObservableMatrix<T> second)
        {
            return ((IStructuralEquatable)this.Content)
                .Equals(second.Content, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Неявное приведение данного типа к типу контента.
        /// </summary>
        /// <param name="matrix"></param>
        public static implicit operator T[,] (ObservableMatrix<T> matrix)
        {
            return matrix._content;
        }
    }
}
