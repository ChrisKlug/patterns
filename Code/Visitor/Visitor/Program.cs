using System;
using System.Collections.Generic;
using System.Drawing;

namespace Visitor
{
    class Program
    {
        static void Main(string[] args)
        {
            var square = new Square(2, 2, 2, 2);
            var circle = new Circle(2, new Point(10, 11));
            var text = new Text("Hello World", 12, new Point(3, 5));

            Console.WriteLine(".: Calculating areas using AreaCalculationVisitor :.\n");

            CalculateArea(square);
            CalculateArea(circle);
            CalculateArea(text);
            CalculateTotalArea(new IAmVisitable[] { square, circle, text });

            Console.WriteLine("\n.: Calculating drawing areas using DrawingAreaCalculationVisitor :.\n");

            CalculateDrawingArea(square);
            CalculateDrawingArea(circle);
            CalculateDrawingArea(text);
            CalculateTotalDrawingArea(new IAmVisitable[] { square, circle, text });

            Console.WriteLine("\n.: Scaling shapes using ScaleAreaByVisitor :.\n");

            Scale(square, 200, x => Console.WriteLine("Square size after scaling: {0} x {1}", x.Width, x.Height));
            Scale(circle, 200, x => Console.WriteLine("Circle radius after scaling: {0}", x.Radius));
            Scale(text, 200, x => Console.WriteLine("Font size after scaling: {0}", x.FontSize));

            Console.WriteLine("\n.: Moving shapes using MoveVisitor :.\n");

            Move(square, new Size(10, 30), x => Console.WriteLine("Square position after moving: {0} x {1}", x.TopLeft.X, x.TopLeft.Y));
            Move(circle, new Size(10, 30), x => Console.WriteLine("Circle position after moving: {0} x {1}", circle.Center.X, circle.Center.Y));
            Move(text, new Size(10, 30), x => Console.WriteLine("Text positon after moving: {0} x {1}", text.Position.X, text.Position.Y));

            Console.ReadKey();
        }


        static void CalculateArea(IAmVisitable visitable)
        {
            var visitor = new AreaCalculationVisitor();
            visitable.Accept(visitor);
            Console.WriteLine("{0} area: {1}", visitable.GetType().Name , visitor.Area);
        }
        static void CalculateTotalArea(IEnumerable<IAmVisitable> visitables)
        {
            var visitor = new AreaCalculationVisitor();
            foreach (var visitable in visitables)
            {
                visitable.Accept(visitor);
            }
            Console.WriteLine("Total area: {0}", visitor.Area);
        }
        static void CalculateDrawingArea(IAmVisitable visitable)
        {
            var visitor = new DrawingAreaCalculationVisitor();
            visitable.Accept(visitor);
            var rect = visitor.Rect;
            Console.WriteLine("Drawing area for {0}: ({1},{2}) ({3},{4})", visitable.GetType().Name, rect.Left, rect.Top, rect.Right, rect.Bottom);
        }
        static void CalculateTotalDrawingArea(IEnumerable<IAmVisitable> visitables)
        {
            var visitor = new DrawingAreaCalculationVisitor();
            foreach (var visitable in visitables)
            {
                visitable.Accept(visitor);
            }
            var rect = visitor.Rect;
            Console.WriteLine("Total drawing area: ({0},{1}) ({2},{3})", rect.Left, rect.Top, rect.Right, rect.Bottom);
        }
        static void Scale<T>(T visitable, double percentage, Action<T> complete) where T: IAmVisitable
        {
            var visitor = new ScaleByAreaVisitor(percentage);
            visitable.Accept(visitor);
            complete(visitable);
        }
        static void Move<T>(T visitable, Size movementSize, Action<T> complete) where T : IAmVisitable
        {
            var visitor = new MoveVisitor(movementSize);
            visitable.Accept(visitor);
            complete(visitable);
        }
    }

    interface IVisitor
    {
        void Visit(Square square);
        void Visit(Circle circle);
        void Visit(Text text);
    }
    interface IAmVisitable
    {
        void Accept(IVisitor visitor);
    }

    class AreaCalculationVisitor : IVisitor
    {
        public void Visit(Square square)
        {
            Area += square.Width * square.Height;
        }

        public void Visit(Circle circle)
        {
            Area += Math.PI * circle.Radius * circle.Radius;
        }

        public void Visit(Text text)
        {
            Area += text.FontSize * text.Content.Length * text.FontSize * .5;
        }

        public void Reset()
        {
            Area = 0;
        }

        public double Area { get; private set; }
    }
    class DrawingAreaCalculationVisitor : IVisitor
    {
        public void Visit(Square square)
        {
            UpdateRect(new Rectangle(square.TopLeft.X, square.TopLeft.Y, (int)Math.Ceiling(square.Width), (int)Math.Ceiling(square.Height)));
        }

        public void Visit(Circle circle)
        {
            var rect = new Rectangle((int)Math.Floor(circle.Center.X - circle.Radius), (int)Math.Floor(circle.Center.Y - circle.Radius), (int)Math.Ceiling(circle.Radius * 2), (int)Math.Ceiling(circle.Radius * 2));
            UpdateRect(rect);
        }

        public void Visit(Text text)
        {
            var width = (int)Math.Ceiling(text.Content.Length * text.FontSize * .5);

            var rect = new Rectangle(text.Position.X, text.Position.Y, width, text.FontSize); ;
            if (text.RightToLeft)
            {
                rect.X = text.Position.X - width;
            }
            UpdateRect(rect);
        }

        public void Reset()
        {
            Rect = Rectangle.Empty;
        }

        private void UpdateRect(Rectangle addedRect)
        {
            Rect = Rect == Rectangle.Empty ? addedRect : Rectangle.Union(Rect, addedRect);
        }

        public Rectangle Rect { get; private set; } = Rectangle.Empty;
    }
    class ScaleByAreaVisitor : IVisitor
    {
        private readonly double _scale;

        public ScaleByAreaVisitor(double percentage)
        {
            _scale = percentage / 100;
        }

        public void Visit(Square square)
        {
            var area = square.Width * square.Height;
            var newArea = area * _scale;
            var rel1 = square.Width / square.Height;
            var rel2 = square.Height / square.Width;
            var x = Math.Sqrt(newArea);
            square.Width = x * rel1;
            square.Height = x * rel2;
        }

        public void Visit(Circle circle)
        {
            var area = Math.PI * circle.Radius * circle.Radius;
            var newArea = area * _scale;
            circle.Radius = Math.Sqrt(newArea / Math.PI);
        }

        public void Visit(Text text)
        {
            text.FontSize *= (int)Math.Ceiling(_scale);
        }
    }
    class MoveVisitor : IVisitor
    {
        private readonly Size _moveBy;

        public MoveVisitor(Size moveBy)
        {
            _moveBy = moveBy;
        }

        public void Visit(Square square)
        {
            var newPosition = new Point(square.TopLeft.X + _moveBy.Width, square.TopLeft.Y + _moveBy.Height);
            square.TopLeft = newPosition;
        }

        public void Visit(Circle circle)
        {
            var newPosition = new Point(circle.Center.X + _moveBy.Width, circle.Center.Y + _moveBy.Height);
            circle.Center = newPosition;
        }

        public void Visit(Text text)
        {
            var newPosition = new Point(text.Position.X + _moveBy.Width, text.Position.Y + _moveBy.Height);
            text.Position = newPosition;
        }
    }

    class Square : IAmVisitable
    {
        public Square(double width, double height, int top, int left)
        {
            Width = width;
            Height = height;
            TopLeft = new Point(left, top);
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        public double Width { get; set; }
        public double Height { get; set; }
        public Point TopLeft { get; set; }
    }

    class Circle : IAmVisitable
    {
        public Circle(double radius, Point center)
        {
            Radius = radius;
            Center = center;
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        public double Radius { get; set; }
        public Point Center { get; set; }
    }

    class Text : IAmVisitable
    {
        public Text(string content, int fontSize, Point position, bool rightToLeft = false)
        {
            Content = content;
            FontSize = fontSize;
            Position = position;
            RightToLeft = rightToLeft;
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        public string Content { get; set; }
        public int FontSize { get; set; }
        public Point Position { get; set; }
        public bool RightToLeft { get; set; }
    }
}
