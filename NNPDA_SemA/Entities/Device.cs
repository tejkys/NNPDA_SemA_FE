namespace NNPDA_SemA.Entities
{
    public class Device
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Sensor> Sensors { get; set; } = new();
    }
}
