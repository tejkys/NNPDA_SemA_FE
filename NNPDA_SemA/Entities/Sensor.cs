namespace NNPDA_SemA.Entities
{
    public class Sensor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DeviceId { get; set; }
        public List<Measure> Measures { get; set; } = new();
    }
}
