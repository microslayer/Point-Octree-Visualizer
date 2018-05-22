using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace LidarServer
{
    class Program
    {
        const string path = @"/Users/pearlleff/Documents/Spring 2018/Geocomputation II/Final Project/Code/Data/";
        const string path_las_file = path + "lidar.las";
        const string path_xyz_file = path + "lidar.txt";
        const string path_normalized_points = path + "normalized_lidar.txt";
        const string path_json_octree = path + "octree.json";
        const string path_json_points = path + "points.json";

        static void Main(string[] args)
        {
            ConvertLasToXyz();
            NormalizeFile();
            CreatePointsJson();

            List<Point> points = GetPointsFromFile(path_normalized_points);
            OctreeNode tree = CreateOctree(points);
            OutputOctants(tree);
        }

        private static void CreatePointsJson()
        {
            using (StreamWriter sw = File.CreateText(path_json_points))
            {
                sw.Write("var points = [");

                using (StreamReader sr = File.OpenText(path_normalized_points))
                {
                    string nextLine;
                    while ((nextLine = sr.ReadLine()) != null)
                    {
                        var attributes = nextLine.Split(',');
                        // assumes attributes are x,y,z
                        double x = Convert.ToDouble(attributes[0]);
                        double y = Convert.ToDouble(attributes[1]);
                        double z = Convert.ToDouble(attributes[2]);
                        sw.WriteLine("{x:" + x + ",y:" + y + ",z: " + z + "},");
                    }
                }

                sw.Write("]");
            }
        }

        private static string ConvertLasToXyz()
        {
            string cmd = "las2txt -i '" + path + "lidar.las' --parse xyzrgbi -o '" + path + "lidar.txt'";
            return ExecuteBashCommand(cmd);
        }

        private static void OutputOctants(OctreeNode tree)
        {
            using (StreamWriter sw = File.CreateText(path_json_octree))
            {
                sw.Write("var data = [");

                int nodesProcessed = 0;

                // search octree and get coordinates of all non-empty octants 
                Stack<OctreeNode> stack = new Stack<OctreeNode>();
                stack.Push(tree);
                while (stack.Count() > 0 /* && nodesProcessed < 1000 */)
                {
                    OctreeNode node = stack.Pop();
                    if (node == null)
                    {
                        continue;
                    }
                    if (!IsNullOrEmpty(node.Children) || node.m_points.Count > 0)
                    {
                        sw.WriteLine(node.m_region + ","); // use to create file 
                        nodesProcessed++;
                    }
                    foreach (OctreeNode octant in node.Children)
                        stack.Push(octant);
                }

                //sw.Write("\b \b"); // remove the last comma 
                sw.Write("]");
            }
        }

        private static void NormalizeFile()
        {
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double minZ = double.MaxValue;
            string[] attributes;
            string s = "";

            using (StreamReader sr = File.OpenText(path_xyz_file))
            {
                while ((s = sr.ReadLine()) != null)
                {
                    attributes = s.Split(',');
                    // assumes attributes are x,y,z
                    double x = Convert.ToDouble(attributes[0]);
                    double y = Convert.ToDouble(attributes[1]);
                    double z = Convert.ToDouble(attributes[2]);
                    if (x < minX)
                        minX = x;
                    if (y < minY)
                        minY = y;
                    if (z < minZ)
                        minZ = z;
                }
            }

            using (StreamWriter sw = File.CreateText(path_normalized_points))
            {
                StreamReader sr2 = new StreamReader(path_xyz_file);
                while ((s = sr2.ReadLine()) != null)
                {
                    attributes = s.Split(',');
                    double x = Convert.ToDouble(attributes[0]);
                    double y = Convert.ToDouble(attributes[1]);
                    double z = Convert.ToDouble(attributes[2]);
                    x = x - minX;
                    y = y - minY;
                    z = z - minZ;
                    sw.WriteLine(string.Join(",", x, y, z));
                    sw.Flush();
                }
                sr2.Close();
            }
        }

        private static List<Point> GetPointsFromFile(string path)
        {
            List<Point> points = new List<Point>();

            using (StreamReader sr = File.OpenText(path))
            {
                string[] attributes;
                string s = "";

                while ((s = sr.ReadLine()) != null)
                {
                    attributes = s.Split(',');
                    // assumes attributes are x,y,z
                    double x = Convert.ToDouble(attributes[0]);
                    double y = Convert.ToDouble(attributes[1]);
                    double z = Convert.ToDouble(attributes[2]);
                    Point p = new Point(x, y, z);
                    points.Add(p);
                }
            }
            return points;
        }

        private static OctreeNode CreateOctree(List<Point> points)
        {
            BoundingBox3D bounds = new BoundingBox3D(getMin(points), getMax(points));
            OctreeNode tree = new OctreeNode(bounds, points);
            tree.Build();
            return tree;
        }

        static string ExecuteBashCommand(string command)
        {
            // from https://stackoverflow.com/questions/23029218/run-bash-commands-from-mono-c-sharp
            command = command.Replace("\"", "\"\"");

            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = "-c \"" + command + "\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();
            proc.WaitForExit();

            return proc.StandardOutput.ReadToEnd();
        }

        private static Vector3 getMin(List<Point> points)
        {
            double minX = points.Min(i => i.Pos.X);
            double minY = points.Min(i => i.Pos.Y);
            double minZ = points.Min(i => i.Pos.Z);
            return new Vector3(minX, minY, minZ);
        }

        private static Vector3 getMax(List<Point> points)
        {
            double maxX = points.Max(i => i.Pos.X);
            double maxY = points.Max(i => i.Pos.Y);
            double maxZ = points.Max(i => i.Pos.Z);
            return new Vector3(maxX, maxY, maxZ);
        }

        public static bool IsNullOrEmpty<T>(T[] array) where T : class
        {
            if (array == null || array.Length == 0)
                return true;
            else
                return array.All(item => item == null);
        }
    }
}
