using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;

namespace SimulatorUwpXaml
{
    public static class BoundaryGenerator
    {
        private static List<BoundingBox> _boundaries;

        public static List<BoundingBox> GenerateBoundingBoxForEachRectangle(List<Rectangle> rectanglesToCreateBoundaryAround)
        {
            List<BoundingBox> boundaries = new List<BoundingBox>();

            foreach(Rectangle rectangle in rectanglesToCreateBoundaryAround)
            {
                Vector3 startBoundary = new Vector3(rectangle.X, rectangle.Y, 0f);
                Vector3 endBoundary =  new Vector3(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, 0f);
                boundaries.Add(new BoundingBox(startBoundary, endBoundary));
            }

            return boundaries;
        }

        // This assumes tiles are read from Left-to-right, top-to-bottom
        public static List<BoundingBox> GenerateCombinedBoundaryBoxes(List<Rectangle> tilesToGenerateBoundaryFor)
        {
            Debug.WriteLine($"Starting to combine {tilesToGenerateBoundaryFor.Count} rectangles to bounding boxes.");
            _boundaries =  new List<BoundingBox>();

            List<Rectangle> untreatedRectangles = AddHorizontalBoundaries(tilesToGenerateBoundaryFor);
            AddVerticalBoundaries(untreatedRectangles);

            Debug.WriteLine($"Total number of bounding boxes: {_boundaries.Count}.");
            return _boundaries;
        }

        private static List<Rectangle> AddHorizontalBoundaries(List<Rectangle> allMapRectangles)
        {
            List<Rectangle> nonHorizontalRectangles = new List<Rectangle>();
            int tileHeight = allMapRectangles[0].Height;
            int tileWidth = allMapRectangles[0].Width;

            int i = 0;
            while (i + 1 < allMapRectangles.Count)
            {
                Rectangle startTile = allMapRectangles[i];

                int lengthToEndBoundry = tileWidth;
                while (i + 1 < allMapRectangles.Count && startTile.Left + lengthToEndBoundry == allMapRectangles[i + 1].Left)
                {
                    lengthToEndBoundry += tileWidth;
                    ++i;
                }

                Vector3 startBoundary = new Vector3(startTile.Left, startTile.Y, 0f);
                Vector3 endBoundary = new Vector3(startTile.X + lengthToEndBoundry, startTile.Y + tileHeight, 0f);

                if (lengthToEndBoundry > tileWidth)
                {
                    _boundaries.Add(new BoundingBox(startBoundary, endBoundary));
                }
                else
                {
                    nonHorizontalRectangles.Add(allMapRectangles[i]);
                    ++i;
                }
            }

            Debug.WriteLine($"Combined {allMapRectangles.Count - nonHorizontalRectangles.Count} rectangles into {_boundaries.Count} horizontal bounding boxes.");
            Debug.WriteLine($"Number non-horizontal rectangles: {nonHorizontalRectangles.Count}.");
            return nonHorizontalRectangles;
        }


        private static void AddVerticalBoundaries(List<Rectangle> nonHorizontalRectangles)
        {
            List<List<Rectangle>> columns = GenerateCollectionsOfAdjacentVerticalRectangles(nonHorizontalRectangles);

            foreach (List<Rectangle> column in columns)
            {
                Rectangle startTile = column[0];
                Rectangle endTile = column.Last();

                Vector3 startBoundary = new Vector3(startTile.Left, startTile.Top, 0f);
                Vector3 endBoundary = new Vector3(endTile.Right, endTile.Bottom, 0f);

                _boundaries.Add(new BoundingBox(startBoundary, endBoundary));
            }

            Debug.WriteLine($"Combined {nonHorizontalRectangles.Count} rectangles vertical bonding boxes (which may consist a single rectangle).");
        }

        private static List<List<Rectangle>> GenerateCollectionsOfAdjacentVerticalRectangles(List<Rectangle> nonHorizontalRectangles)
        {
            List<List<Rectangle>> columns = new List<List<Rectangle>>();

            foreach (Rectangle rectangle in nonHorizontalRectangles)
            {
                bool startOfNewColumn = true;
                foreach (List<Rectangle> column in columns)
                {
                    if (column.Last().Bottom == rectangle.Top && 
                        column.Last().Left == rectangle.Left)
                    {
                        column.Add(rectangle);
                        startOfNewColumn = false;
                        break;
                    }              
                }
                if (startOfNewColumn)
                {
                    columns.Add(new List<Rectangle>(){rectangle});
                }   
            }

            return columns;
        }
    }
}
