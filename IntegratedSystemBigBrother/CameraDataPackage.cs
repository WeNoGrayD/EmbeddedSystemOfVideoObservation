using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace IntegratedSystemBigBrother
{
    /// <summary>
    /// Пакет данных о событии, зафиксированных камерой.
    /// </summary>
    public abstract class CameraDataPackage
    {
        public enum EventDescription
        {
            StandardSituation,
            EmployeeArrival,
            EmployeeDeparture,
            OutsiderOnObject,
            RecordingOngoing
        }

        public DateTime TimeStamp { get; protected set; }

        public readonly EventDescription MessageImportance;

        public string Message { get { return GetMessage(); } }

        public string CameraName { get; set; }

        public readonly bool IsFirstInSeries;

        protected CameraDataPackage(
            DateTime timeStamp, 
            EventDescription messageImportance,
            bool isFirstInSeries)
        {
            TimeStamp = timeStamp;
            MessageImportance = messageImportance;
            IsFirstInSeries = isFirstInSeries;
        }

        public abstract void Accept(ICameraDataPackageVisitor reader);

        public abstract string GetMessage();
    }

    /// <summary>
    /// Интерфейс-посетитель, работающий с пакетом данных от камеры. 
    /// </summary>
    public interface ICameraDataPackageVisitor
    {
        void Visit(CameraStandardSituationDataPackage package);
        void Visit(CameraEmployeeArrivalDataPackage package);
        void Visit(CameraEmployeeDepartureDataPackage package);
        void Visit(CameraOutsiderOnObjectDataPackage package);
    }

    /// <summary>
    /// Пакет данных, свидетельствующий об отсутствии событий.
    /// </summary>
    public class CameraStandardSituationDataPackage : CameraDataPackage
    {
        public CameraStandardSituationDataPackage(DateTime timeStamp, bool isFirstInSeries)
            : base(timeStamp, EventDescription.StandardSituation, isFirstInSeries)
        { }

        public override void Accept(ICameraDataPackageVisitor reader)
        {
            reader.Visit(this);
        }

        public override string GetMessage() => "Здесь не на что смотреть.";
    }

    /// <summary>
    /// Пакет данных, свидетельствующий о событии, случившемся с неким сотрудником.
    /// </summary>
    public abstract class CameraEmployeeEventDataPackage : CameraDataPackage
    {
        public readonly string EmployeeName;

        protected CameraEmployeeEventDataPackage(
            DateTime timeStamp, 
            EventDescription messageImportance,
            bool isFirstInSeries, 
            string employeeName)
            : base(timeStamp, messageImportance, isFirstInSeries)
        {
            EmployeeName = employeeName;
        }
    }

    /// <summary>
    /// Пакет данных, свидетельствующий о прибытии сотрудника.
    /// </summary>
    public class CameraEmployeeArrivalDataPackage : CameraEmployeeEventDataPackage
    {
        public CameraEmployeeArrivalDataPackage(
            DateTime timeStamp, 
            bool isFirstInSeries, 
            string employeeName)
            : base(timeStamp, EventDescription.EmployeeArrival, isFirstInSeries, employeeName)
        { }

        public override void Accept(ICameraDataPackageVisitor reader)
        {
            reader.Visit(this);
        }

        public override string GetMessage() => $"Прибыл сотрудник {EmployeeName}.";
    }

    /// <summary>
    /// Пакет данных, свидетельствующий об уходе сотрудника.
    /// </summary>
    public class CameraEmployeeDepartureDataPackage : CameraEmployeeEventDataPackage
    {
        public CameraEmployeeDepartureDataPackage(DateTime timeStamp, bool isFirstInSeries, string employeeName)
            : base(timeStamp, EventDescription.EmployeeDeparture, isFirstInSeries, employeeName)
        { }

        public override void Accept(ICameraDataPackageVisitor reader)
        {
            reader.Visit(this);
        }

        public override string GetMessage() => $"Отбыл сотрудник {EmployeeName}.";
    }

    /// <summary>
    /// Пакет данных, свидетельствующий о появлении постороннего.
    /// </summary>
    public class CameraOutsiderOnObjectDataPackage : CameraDataPackage
    {
        public CameraOutsiderOnObjectDataPackage(DateTime timeStamp, bool isFirstInSeries)
            : base(timeStamp, EventDescription.OutsiderOnObject, isFirstInSeries)
        { }

        public override void Accept(ICameraDataPackageVisitor reader)
        {
            reader.Visit(this);
        }

        public override string GetMessage() => $"Замечен посторонний!";
    }

    /*
    /// <summary>
    /// Пакет данных, свидетельствующий о ведении съёмки.
    /// </summary>
    public class CameraRecordingOngoingDataPackage : CameraDataPackage
    {
        public readonly bool JustStarted;

        public readonly Bitmap Snapshot;

        public CameraRecordingOngoingDataPackage(DateTime timeStamp, bool justStarted, Bitmap snapshot)
            : base(timeStamp, EventDescription.RecordingOngoing)
        {
            JustStarted = justStarted;
            Snapshot = snapshot;
        }

        public override void Accept(ICameraDataPackageVisitor reader)
        {
            reader.Visit(this);
        }

        public override string GetMessage() => $"Ведётся съёмка.";
    }
    */
}
