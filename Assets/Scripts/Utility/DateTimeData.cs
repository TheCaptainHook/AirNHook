using System;



[Serializable]
public class DateTimeData
{
    public int year;
    public int month;
    public int day;
    public int hour;
    public int minute;
    public int second;

    public DateTimeData(DateTime dateTime)
    {
        year = dateTime.Year;
        month = dateTime.Month;
        day = dateTime.Day;
        hour = dateTime.Hour;
        minute = dateTime.Minute;
        second = dateTime.Second;
    }
}
