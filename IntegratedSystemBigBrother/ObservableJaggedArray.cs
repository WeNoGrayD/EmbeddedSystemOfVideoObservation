using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;

namespace IntegratedSystemBigBrother
{
    public class ObservableJaggedArray<T> :
            IEnumerable, IEnumerable<T>,
            INotifyCollectionChanged, INotifyPropertyChanged
    {
        private T[][] _content;
        /// <summary>
        /// Контент матрицы.
        /// </summary>
        public T[][] Content
        {
            get { return _content; }
            private set
            {
                _content = value;
                OnPropertyChanged(nameof(Content));
            }
        }

        /// <summary>
        /// Константа названия свойства индексатора.
        /// </summary>
        public const string IndexerName = "Item[]";

        /// <summary>
        /// Индексатор, дающий доступ к рядам матрицы.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public T[] this[int i]
        {
            get { return _content[i]; }
            set
            {
                Content[i] = value;
                OnPropertyChanged(IndexerName);
            }
        }

        /// <summary>
        /// Индексатор, дающий доступ к значениям матрицы по индексу.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public T this[int i, int j]
        {
            get { return _content[i][j]; }
            set
            {
                if (!Content[i][j]?.Equals(value) ?? true)
                {
                    Content[i][j] = value;
                    OnPropertyChanged(IndexerName);
                }
            }
        }

        /// <summary>
        /// Размер массива массивов. Длиной второго измерения считается .
        /// </summary>
        public readonly (int Rows, int Colums) Size;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="size1"></param>
        /// <param name="size2"></param>
        public ObservableJaggedArray(int size1, int size2) : base()
        {
            _content = new T[size1][];
            for (int i = 0; i < size1; i++)
                _content[i] = new T[size2];

            Size = (size1, size2);
        }

        /// <summary>
        /// Копирование контента матрицы в другую матрицу.
        /// </summary>
        /// <param name="receiver"></param>
        public void CopyTo(T[][] receiver)
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
        /// Сравнение с другим массивом массивов.
        /// </summary>
        /// <param name="otherJaggedArrayRow"></param>
        /// <returns></returns>
        public bool RowEquals(int row, T[] otherJaggedArrayRow)
        {
            return ((IStructuralEquatable)this.Content[row])
                .Equals(otherJaggedArrayRow, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Сравнение с другим массивом массивов.
        /// </summary>
        /// <param name="otherJaggedArray"></param>
        /// <returns></returns>
        public bool Equals(T[][] otherJaggedArray)
        {
            return ((IStructuralEquatable)this.Content)
                .Equals(otherJaggedArray, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Неявное приведение данного типа к типу контента.
        /// </summary>
        /// <param name="jaggedArray"></param>
        public static implicit operator T[][] (ObservableJaggedArray<T> jaggedArray)
        {
            return jaggedArray.Content;
        }
    }
}
