namespace LidarServer
{
    public class Octree
    {
        OctreeNode m_root;
        BoundingBox3D m_region; 

        public Octree()
        {
            m_root = null;
        }
    }
}