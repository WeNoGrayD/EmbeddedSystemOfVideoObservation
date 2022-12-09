using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntegratedSystemBigBrother;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace IntegratedSystemBigBrotherTests
{
    [TestClass]
    public class ObservableJaggedarrayTest
    {
        ObservableJaggedArray<byte> image;
        
        [TestInitialize]
        public void InitJaggedArray()
        {
            image = new ObservableJaggedArray<byte>(480, 640);
            FulfillImageWithRandomValues(0);
        }

        public void FulfillImageWithRandomValues(int seed)
        {
            (int rows, int columns) = image.Size;
            Random randGen = new Random(seed);
            byte[] buffer = new byte[columns];

            for (int i = 0; i < rows; i++)
            {
                randGen.NextBytes(buffer);
                image[i] = buffer;
            }
        }

        [TestMethod]
        public void Copying()
        {
            (int rows, int columns) = image.Size;
            ObservableJaggedArray<byte> image2 = new ObservableJaggedArray<byte>(rows, columns);
            image.CopyTo(image2);
            Assert.IsTrue(image.Equals(image2));
        }

        class ObservableJaggedArraySubscriber<T>
        {
            public enum ListenedState
            {
                NOT_CHANGED,
                CHANGED
            }

            public ListenedState ListenedArrayState { get; private set; }

            private int _listenedRow;
            public T[] Data;

            public void SubscribeOn(ObservableJaggedArray<T> listened, 
                int listenedRow, int rowWidth)
            {
                listened.CollectionChanged += ListenedCollectionChanged;
                listened.PropertyChanged += ListenedPropertyChanged;

                ListenedArrayState = ListenedState.NOT_CHANGED;
                _listenedRow = listenedRow;
                Data = new T[rowWidth];
                UpdateData(listened, listenedRow);
            }

            public void ListenedCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                ListenedArrayState = ListenedState.CHANGED;
            }

            public void ListenedPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == ObservableJaggedArray<T>.IndexerName)
                {
                    UpdateData((ObservableJaggedArray<T>)sender, _listenedRow);
                }
            }

            private void UpdateData(ObservableJaggedArray<T> source, int row)
            {
                source[row].CopyTo(Data, 0);
            }
        }

        [TestMethod]
        public void Subscribing()
        {
            ObservableJaggedArraySubscriber<byte> sub = 
                new ObservableJaggedArraySubscriber<byte>();
            int listenedRow = 10;
            (int rows, int columns) = image.Size;
            sub.SubscribeOn(image, listenedRow, columns);
            Console.WriteLine(image[listenedRow].Take(10).ToArray());
            Console.WriteLine(sub.Data.Take(10).ToArray());
            Assert.IsTrue(image.RowEquals(listenedRow, sub.Data));
            FulfillImageWithRandomValues(5);
            Console.WriteLine(image[listenedRow].Take(10).ToArray());
            Console.WriteLine(sub.Data.Take(10).ToArray());
            Assert.IsTrue(image.RowEquals(listenedRow, sub.Data));
        }
    }
}
