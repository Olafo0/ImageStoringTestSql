using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace ImageStoringTest
{
    public partial class Form1 : Form
    {

        string connectionString = "Data Source=DESKTOP-DNB9KRF;Initial Catalog=ImageDatabaseTest;Integrated Security=True;";
        public Form1()
        {

            InitializeComponent();
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Image image = Image.FromFile("C:\\Users\\user\\Desktop\\Coding part2\\ImageStoringTest\\ImageStoringTest\\SayWhatDog.jpg");

            // Convert the image to a byte array
            byte[] imageData;

            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                imageData = ms.ToArray();
            }
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();

                // SQL query with a parameter for the VARBINARY(MAX) 
                string query = "INSERT INTO ImageStore(Images) VALUES(@ImageData)";

                using (SqlCommand cmd = new SqlCommand(query, cnn))
                {
                    // Add parameter for the byte array
                    cmd.Parameters.Add("@ImageData", System.Data.SqlDbType.VarBinary, -1).Value = imageData;

                    // Execute the query
                    cmd.ExecuteNonQuery();

                    Console.WriteLine("Data inserted successfully.");
                }
            }
        }

        // Making the byte from the database into an image
        private void button2_Click(object sender, EventArgs e)
        {

            List<byte[]> list = new List<byte[]>();

            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                string Query = "SELECT * FROM ImageStore";
                using (SqlCommand cmd = new SqlCommand(Query, cnn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Object ImageData = reader["Images"];
                            byte[] tempImg = (byte[])ImageData;
                            list.Add(tempImg);
                        }
                    }
                }
            }

            byte[] imgByte = list[0];
            using (MemoryStream ms = new MemoryStream(imgByte))
            {
                Image tempImage = Image.FromStream(ms);
                pictureBox1.Image = tempImage;
            }
        }
    }

}
