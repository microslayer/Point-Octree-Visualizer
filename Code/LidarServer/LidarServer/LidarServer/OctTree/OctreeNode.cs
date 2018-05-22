// built with the help of 
// https://www.gamedev.net/articles/programming/general-and-gameplay-programming/introduction-to-octrees-r3529/

using System;
using System.Collections.Generic;

namespace LidarServer
{
    public class OctreeNode
    {
        public BoundingBox3D m_region;
        public List<Point> m_points;
       
        public OctreeNode Parent;
        public OctreeNode[] Children = new OctreeNode[8];

        const int MIN_SIZE = 5;

        public OctreeNode(BoundingBox3D region, List<Point> points)
        {
            m_region = region;
            m_points = points; 
        }

        // builds a tree from scratch; should be called once 
        public void Build()
        {
            if (m_points.Count <= 1) // leaf node 
                return;

            Vector3 dimensions = m_region.Max - m_region.Min;

            if (dimensions == Vector3.Zero)
            {
                throw new Exception("dimensions is zero"); 
                // FindEnclosingCube();
                // dimensions = m_region.Max - m_region.Min;
            }

            // check if dimensions of box are greater than the minimum dimension
            if (dimensions.X <= MIN_SIZE && dimensions.Y <= MIN_SIZE && dimensions.Z <= MIN_SIZE)
            {
                return;
            }

            // divide region into 8 octants 
            BoundingBox3D[] octants = DivideIntoOctants(m_region, dimensions);

            List<Point>[] octantPoints = new List<Point>[8];

            for (int i = 0; i < 8; i++)
                octantPoints[i] = new List<Point>();  

            // list containing all of the objects which got moved down the tree and can be delisted from this node.
            List<Point> delist = new List<Point>();

            foreach (Point p in m_points)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (octants[i].Contains(p.Pos))
                    {
                        octantPoints[i].Add(p);
                        delist.Add(p);
                        break;
                    }
                }
            }

            // create child nodes with relevant points in bounding region
            for (int a = 0; a < 8; a++)
            {
                if (octantPoints[a].Count != 0)
                {
                    Children[a] = CreateNode(octants[a], octantPoints[a]);
                    Children[a].Build();
                }
            }

            // remove delisted nodes 
            foreach (Point p in delist)
                m_points.Remove(p);
        }

        private BoundingBox3D[] DivideIntoOctants(BoundingBox3D region, Vector3 dimensions)
        {
            Vector3 half = dimensions / 2.0f;
            Vector3 center = region.Min + half;

            // divide region into 8 octants 
            BoundingBox3D[] octant = new BoundingBox3D[8];
            octant[0] = new BoundingBox3D(region.Min, center);
            octant[1] = new BoundingBox3D(new Vector3(center.X, region.Min.Y, region.Min.Z), new Vector3(region.Max.X, center.Y, center.Z));
            octant[2] = new BoundingBox3D(new Vector3(center.X, region.Min.Y, center.Z), new Vector3(region.Max.X, center.Y, region.Max.Z));
            octant[3] = new BoundingBox3D(new Vector3(region.Min.X, region.Min.Y, center.Z), new Vector3(center.X, center.Y, region.Max.Z));
            octant[4] = new BoundingBox3D(new Vector3(region.Min.X, center.Y, region.Min.Z), new Vector3(center.X, region.Max.Y, center.Z));
            octant[5] = new BoundingBox3D(new Vector3(center.X, center.Y, region.Min.Z), new Vector3(region.Max.X, region.Max.Y, center.Z));
            octant[6] = new BoundingBox3D(center, region.Max);
            octant[7] = new BoundingBox3D(new Vector3(region.Min.X, center.Y, center.Z), new Vector3(center.X, region.Max.Y, region.Max.Z));

            return octant;
        }

        private void Insert(Point point)
        {
            // if the current node is an empty leaf node, just insert it and leave 
            if (m_points.Count <= 1)
            {
                m_points.Add(point);
                return;
            }

            Vector3 dimensions = m_region.Max - m_region.Min;

            // check if the dimensions of the box are greater than the minimum dimensions
            if (dimensions.X <= MIN_SIZE && dimensions.Y <= MIN_SIZE && dimensions.Z <= MIN_SIZE)
            {
                m_points.Add(point);
                return;
            }

            BoundingBox3D[] octants = FindOrCreateOctants(Children, m_region, dimensions); 

            if (m_region.Contains(point.Pos))
            {
                bool found = false;
                // Try to place the object into a child node. If we can't fit it in a child node, then we insert it into the current node object list.
                for (int a = 0; a < 8; a++)
                {
                    // test if object is fully contained within a quadrant
                    if (octants[a].Contains(point.Pos))
                    {
                        if (Children[a] != null)
                            Children[a].Insert(point); // add the item into that tree, let the child tree figure out what to do with it
                        else
                        {
                            Children[a] = CreateNode(octants[a], point); // create a new tree node with the item
                        }
                        found = true;
                    }
                }

                if (!found) 
                    m_points.Add(point);
            }
            else
            {
                // The item either lies outside of the enclosed bounding box or it is intersecting it. Either way, need to rebuild
                // the entire tree by enlarging the containing bounding box
                Build();
            }
        }

        private BoundingBox3D[] FindOrCreateOctants(OctreeNode[] children, BoundingBox3D region, Vector3 dimensions)
        {
            Vector3 half = dimensions / 2.0f;
            Vector3 center = region.Min + half;

            BoundingBox3D[] octants = new BoundingBox3D[8];
            octants[0] = (children[0] != null) ? children[0].m_region : new BoundingBox3D(region.Min, center);
            octants[1] = (children[1] != null) ? children[1].m_region : new BoundingBox3D(new Vector3(center.X, region.Min.Y, region.Min.Z), new Vector3(region.Max.X, center.Y, center.Z));
            octants[2] = (children[2] != null) ? children[2].m_region : new BoundingBox3D(new Vector3(center.X, region.Min.Y, center.Z), new Vector3(region.Max.X, center.Y, region.Max.Z));
            octants[3] = (children[3] != null) ? children[3].m_region : new BoundingBox3D(new Vector3(region.Min.X, region.Min.Y, center.Z), new Vector3(center.X, center.Y, region.Max.Z));
            octants[4] = (children[4] != null) ? children[4].m_region : new BoundingBox3D(new Vector3(region.Min.X, center.Y, region.Min.Z), new Vector3(center.X, region.Max.Y, center.Z));
            octants[5] = (children[5] != null) ? children[5].m_region : new BoundingBox3D(new Vector3(center.X, center.Y, region.Min.Z), new Vector3(region.Max.X, region.Max.Y, center.Z));
            octants[6] = (children[6] != null) ? children[6].m_region : new BoundingBox3D(center, region.Max);
            octants[7] = (children[7] != null) ? children[7].m_region : new BoundingBox3D(new Vector3(region.Min.X, center.Y, center.Z), new Vector3(center.X, region.Max.Y, region.Max.Z));

            return octants; 
        }

        private OctreeNode CreateNode(BoundingBox3D region, List<Point> points)
        {
            if (points.Count == 0)
                return null;
            OctreeNode ret = new OctreeNode(region, points);
            ret.Parent = this;
            return ret;
        }

        private OctreeNode CreateNode(BoundingBox3D region, Point p)
        {
            List<Point> points = new List<Point>(1); //sacrifice potential CPU time for a smaller memory footprint
            points.Add(p);
            OctreeNode node = new OctreeNode(region, points);
            node.Parent = this;
            return node;
        }

    }
}