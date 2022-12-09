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

        public readonly DateTime TimeStamp;

        public readonly EventDescription MessageImportance;

        public string CameraName { get; set; }

        protected CameraDataPackage(DateTime timeStamp, EventDescription messageImportance)
        {
            TimeStamp = timeStamp;
            MessageImportance = messageImportance;
        }

        public abstract void Accept(ICameraDataPackageVisitor reader);

        public abstract string GetMessage();
    }

    /// <summary>
    /// Интерфейс-посетитель, работающий с пакетом данных от камеры. 
    /// </summary>
    public interface ICameraDataPackageVisitor
    {
        void Visit(CameraDataPackage package);
    }

    /// <summary>
    /// Пакет данных, свидетельствующий об отсутствии событий.
    /// </summary>
    public class CameraStandardSituationDataPackage : CameraDataPackage
    {
        public CameraStandardSituationDataPackage(DateTime timeStamp)
            : base(timeStamp, EventDescription.StandardSituation)
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
            DateTime timeStamp, EventDescription messageImportance, string employeeName)
            : base(timeStamp, messageImportance)
        {
            EmployeeName = employeeName;
        }
    }

    /// <summary>
    /// Пакет данных, свидетельствующий о прибытии сотрудника.
    /// </summary>
    public class CameraEmployeeArrivalDataPackage : CameraEmployeeEventDataPackage
    {
        public CameraEmployeeArrivalDataPackage(DateTime timeStamp, string employeeName)
            : base(timeStamp, EventDescription.EmployeeArrival, employeeName)
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
        public CameraEmployeeDepartureDataPackage(DateTime timeStamp, string employeeName)
            : base(timeStamp, EventDescription.EmployeeDeparture, employeeName)
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
        public CameraOutsiderOnObjectDataPackage(DateTime timeStamp)
            : base(timeStamp, EventDescription.OutsiderOnObject)
        { }

        public override void Accept(ICameraDataPackageVisitor reader)
        {
            reader.Visit(this);
        }

        public override string GetMessage() => $"Замечен посторонний!";
    }

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
}
