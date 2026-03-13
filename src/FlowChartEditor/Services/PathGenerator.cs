using System;
using System.Windows;
using System.Windows.Media;

namespace FlowChartEditor.Services
{
    public static class PathGenerator
    {
        public enum ConnectionType
        {
            Straight,
            Bezier,
            Orthogonal
        }

        /// <summary>
        /// Creates an orthogonal path with rounded corners connecting two points
        /// </summary>
        public static PathGeometry CreateOrthogonalPath(Point start, Point end, double cornerRadius = 10)
        {
            var geometry = new PathGeometry();
            var figure = new PathFigure();

            figure.StartPoint = start;

            // Calculate the middle points for orthogonal path
            // We'll use a simple L-shaped approach with possible horizontal/vertical segments
            double midX = start.X + (end.X - start.X) / 2;
            
            // Create points for the orthogonal path
            // Strategy: start -> horizontal -> vertical -> end
            Point p1, p2;
            
            // Determine the intermediate points based on relative positions
            if (Math.Abs(end.X - start.X) > 2 * cornerRadius && Math.Abs(end.Y - start.Y) > 2 * cornerRadius)
            {
                // Standard L-shape
                p1 = new Point(start.X < end.X ? start.X + Math.Abs(end.X - start.X) / 2 : start.X - Math.Abs(end.X - start.X) / 2, start.Y);
                p2 = new Point(p1.X, start.Y < end.Y ? start.Y + Math.Abs(end.Y - start.Y) / 2 : start.Y - Math.Abs(end.Y - start.Y) / 2);
            }
            else
            {
                // Use a more complex path with additional segments for spacing
                double offsetX = Math.Sign(end.X - start.X) * Math.Max(cornerRadius, Math.Abs(end.X - start.X) / 3);
                double offsetY = Math.Sign(end.Y - start.Y) * Math.Max(cornerRadius, Math.Abs(end.Y - start.Y) / 3);
                
                p1 = new Point(start.X + offsetX, start.Y);
                p2 = new Point(p1.X, end.Y - offsetY);
            }

            // Create path segments with rounded corners using arcs
            if (cornerRadius > 0)
            {
                // From start to p1 (horizontal)
                if (Math.Abs(p1.X - start.X) > 0.1)
                {
                    var lineSegment = new LineSegment(p1, true);
                    figure.Segments.Add(lineSegment);
                }

                // From p1 to p2 with rounded corner at p1
                if (Math.Abs(p2.Y - p1.Y) > 0.1)
                {
                    // Draw arc at p1 if we need to round the corner
                    var cornerPoint1 = new Point(p1.X, p1.Y);
                    
                    // Calculate direction vectors for the arc
                    Point prevPoint = p1.X != start.X ? new Point(p1.X - Math.Sign(p1.X - start.X) * cornerRadius, p1.Y) : start;
                    Point nextPoint = p1.Y != p2.Y ? new Point(p1.X, p1.Y - Math.Sign(p1.Y - p2.Y) * cornerRadius) : p2;
                    
                    // Adjust points to ensure proper arc placement
                    Point arcStart = new Point(
                        p1.X - (p1.X == prevPoint.X ? 0 : Math.Sign(p1.X - prevPoint.X) * cornerRadius),
                        p1.Y - (p1.Y == prevPoint.Y ? 0 : Math.Sign(p1.Y - prevPoint.Y) * cornerRadius)
                    );
                    
                    Point arcEnd = new Point(
                        p1.X - (p1.X == nextPoint.X ? 0 : Math.Sign(p1.X - nextPoint.X) * cornerRadius),
                        p1.Y - (p1.Y == nextPoint.Y ? 0 : Math.Sign(p1.Y - nextPoint.Y) * cornerRadius)
                    );
                    
                    if (Math.Abs(arcStart.X - p1.X) >= 0.1 || Math.Abs(arcStart.Y - p1.Y) >= 0.1)
                    {
                        var lineToArcStart = new LineSegment(arcStart, true);
                        figure.Segments.Add(lineToArcStart);
                    }
                    
                    var arcSegment = new ArcSegment
                    {
                        Point = arcEnd,
                        Size = new Size(cornerRadius, cornerRadius),
                        RotationAngle = 0,
                        IsLargeArc = false,
                        SweepDirection = SweepDirection.Counterclockwise
                    };
                    
                    // Adjust sweep direction based on actual direction of turn
                    if ((start.X < p1.X && p1.Y < p2.Y) || (start.X > p1.X && p1.Y > p2.Y))
                        arcSegment.SweepDirection = SweepDirection.Clockwise;
                    
                    figure.Segments.Add(arcSegment);
                    
                    // Line from arc end to p2
                    var lineFromArc = new LineSegment(p2, true);
                    figure.Segments.Add(lineFromArc);
                }
                else
                {
                    var lineSegment = new LineSegment(p2, true);
                    figure.Segments.Add(lineSegment);
                }

                // From p2 to end
                if (Math.Abs(end.X - p2.X) > 0.1 || Math.Abs(end.Y - p2.Y) > 0.1)
                {
                    // Add rounded corner from p2 to end if needed
                    Point prevPoint = p2.Y != p1.Y ? p1 : new Point(end.X, p2.Y);
                    Point arcStart = new Point(
                        p2.X - (Math.Abs(p2.X - prevPoint.X) > 0.1 ? Math.Sign(p2.X - prevPoint.X) * cornerRadius : 0),
                        p2.Y - (Math.Abs(p2.Y - prevPoint.Y) > 0.1 ? Math.Sign(p2.Y - prevPoint.Y) * cornerRadius : 0)
                    );
                    
                    Point arcEnd = new Point(
                        end.X - (Math.Abs(end.X - p2.X) > 0.1 ? Math.Sign(end.X - p2.X) * cornerRadius : 0),
                        end.Y - (Math.Abs(end.Y - p2.Y) > 0.1 ? Math.Sign(end.Y - p2.Y) * cornerRadius : 0)
                    );
                    
                    if (Math.Abs(arcStart.X - p2.X) >= 0.1 || Math.Abs(arcStart.Y - p2.Y) >= 0.1)
                    {
                        var lineToArcStart = new LineSegment(arcStart, true);
                        figure.Segments.Add(lineToArcStart);
                    }
                    
                    if (Math.Abs(arcEnd.X - arcStart.X) >= 0.1 || Math.Abs(arcEnd.Y - arcStart.Y) >= 0.1)
                    {
                        var arcSegment = new ArcSegment
                        {
                            Point = arcEnd,
                            Size = new Size(cornerRadius, cornerRadius),
                            RotationAngle = 0,
                            IsLargeArc = false,
                            SweepDirection = SweepDirection.Counterclockwise
                        };
                        
                        // Determine sweep direction based on turn direction
                        if ((p2.X < arcEnd.X && p2.Y > end.Y) || (p2.X > arcEnd.X && p2.Y < end.Y))
                            arcSegment.SweepDirection = SweepDirection.Clockwise;
                            
                        figure.Segments.Add(arcSegment);
                    }
                    
                    var lineToFinish = new LineSegment(end, true);
                    figure.Segments.Add(lineToFinish);
                }
            }
            else
            {
                // Without rounded corners - straight segments
                var line1 = new LineSegment(p1, true);
                var line2 = new LineSegment(p2, true);
                var line3 = new LineSegment(end, true);
                
                figure.Segments.Add(line1);
                figure.Segments.Add(line2);
                figure.Segments.Add(line3);
            }

            geometry.Figures.Add(figure);
            return geometry;
        }

        /// <summary>
        /// Creates a smooth Bézier curve between two points
        /// </summary>
        public static PathGeometry CreateBezierPath(Point start, Point end)
        {
            var geometry = new PathGeometry();
            var figure = new PathFigure();
            
            figure.StartPoint = start;
            
            // Calculate control points for a nice curve
            double distance = Math.Abs(end.X - start.X);
            Point controlPoint1 = new Point(start.X + distance / 3, start.Y);
            Point controlPoint2 = new Point(end.X - distance / 3, end.Y);
            
            var bezierSegment = new BezierSegment(controlPoint1, controlPoint2, end, true);
            figure.Segments.Add(bezierSegment);
            
            geometry.Figures.Add(figure);
            return geometry;
        }

        /// <summary>
        /// Creates a straight line between two points
        /// </summary>
        public static PathGeometry CreateStraightPath(Point start, Point end)
        {
            var geometry = new PathGeometry();
            var figure = new PathFigure();
            
            figure.StartPoint = start;
            var lineSegment = new LineSegment(end, true);
            figure.Segments.Add(lineSegment);
            
            geometry.Figures.Add(figure);
            return geometry;
        }

        /// <summary>
        /// Calculates the position of a connection point on a node
        /// </summary>
        public static Point GetConnectionPointPosition(dynamic node, string pointName)
        {
            // Assuming node has Position (Point) and Size (Size) properties
            var position = node.Position;
            var size = node.Size;
            
            return pointName.ToLower() switch
            {
                "top" => new Point(position.X + size.Width / 2, position.Y),
                "bottom" => new Point(position.X + size.Width / 2, position.Y + size.Height),
                "left" => new Point(position.X, position.Y + size.Height / 2),
                "right" => new Point(position.X + size.Width, position.Y + size.Height / 2),
                "top-left" => new Point(position.X, position.Y),
                "top-right" => new Point(position.X + size.Width, position.Y),
                "bottom-left" => new Point(position.X, position.Y + size.Height),
                "bottom-right" => new Point(position.X + size.Width, position.Y + size.Height),
                _ => new Point(position.X + size.Width / 2, position.Y + size.Height / 2) // center
            };
        }
    }
}