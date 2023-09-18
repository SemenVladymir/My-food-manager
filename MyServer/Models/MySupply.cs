namespace MyServer.Models
{
    public class MySupply
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public float Weight { get; set; }
        public DateTime DateOfEnd { get; set; }

        public MySupply() 
        {
            Id = 0;
            ProductId = 0;
            Weight = 0;
            DateOfEnd = DateTime.Now;
        }

        public MySupply(int productId, float weight, DateTime dateOfEnd)
        {
            ProductId = productId;
            Weight = weight;
            DateOfEnd = dateOfEnd;
        }
    }
}
