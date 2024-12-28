namespace NNPDA_SemA.Entities;

public class Measure
{
    public int Id { get; set; }
    public int Value { get; set; }
    public int SensorId { get; set; }

    public Measure()
    {
    }

    public Measure(int id, int value, int sensorId)
    {
        Id = id;
        Value = value;
        SensorId = sensorId;
    }
}