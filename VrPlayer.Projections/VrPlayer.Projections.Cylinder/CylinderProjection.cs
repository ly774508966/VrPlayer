﻿using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using VrPlayer.Contracts.Projections;

namespace VrPlayer.Projections.Cylinder
{
    [Export(typeof(IProjection))]
    public class CylinderProjection : ProjectionBase, IProjection
    {
        Point3D _center;
        double _radius = 1;

        public static readonly DependencyProperty ScaleProperty =
             DependencyProperty.Register("Scale", typeof(double),
             typeof(CylinderProjection), new FrameworkPropertyMetadata(1D));
        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        public static readonly DependencyProperty SlicesProperty =
             DependencyProperty.Register("Slices", typeof(int),
             typeof(CylinderProjection), new FrameworkPropertyMetadata(32));
        public int Slices
        {
            get { return (int)GetValue(SlicesProperty); }
            set { SetValue(SlicesProperty, value); }
        }

        public static readonly DependencyProperty StacksProperty =
             DependencyProperty.Register("Stacks", typeof(int),
             typeof(CylinderProjection), new FrameworkPropertyMetadata(16));
        public int Stacks
        {
            get { return (int)GetValue(StacksProperty); }
            set { SetValue(StacksProperty, value); }
        }

        public Point3D Center
        {
            get { return _center; }
            set
            {
                _center = value;
                OnPropertyChanged("Center");
            }
        }

        public double Radius
        {
            get { return _radius; }
            set
            {
                _radius = value;
                OnPropertyChanged("Radius");
            }
        }

        public new Vector3D CameraLeftPosition
        {
            get
            {
                return new Vector3D(_radius, 0, 0);
            }
        }

        public new Vector3D CameraRightPosition
        {
            get
            {
                return new Vector3D(-_radius, 0, 0);
            }
        }

        public override Point3DCollection Positions
        {
            get
            {
                var positions = new Point3DCollection();

                //LEFT
                for (int stack = 0; stack <= Stacks; stack++)
                {
                    double y = -(stack*(Radius/Stacks))*2 + Radius;
                    double ratio = -Radius;

                    for (int slice = 0; slice <= Slices; slice++)
                    {
                        double theta = slice * 2 * Math.PI / Slices;
                        double x = ratio * Math.Sin(theta) + Radius;
                        double z = ratio * Math.Cos(theta);

                        var normal = new Vector3D(x, y*Scale, z);
                        positions.Add(normal + Center);
                    }
                }

                //RIGH
                for (int stack = 0; stack <= Stacks; stack++)
                {
                    double y = -(stack*(Radius/Stacks))*2 + Radius;
                    double ratio = -Radius;

                    for (int slice = 0; slice <= Slices; slice++)
                    {
                        double theta = slice * 2 * Math.PI / Slices;
                        double x = ratio * Math.Sin(theta) - Radius;
                        double z = ratio * Math.Cos(theta);

                        var normal = new Vector3D(x, y*Scale, z);
                        positions.Add(normal + Center);
                    }
                }

                return positions;
            }
        }

        public override Int32Collection TriangleIndices
        {
            get
            {
                var triangleIndices = new Int32Collection();

                //LEFT
                for (int stack = 0; stack < Stacks; stack++)
                {
                    int top = (stack + 0) * (Slices + 1);
                    int bot = (stack + 1) * (Slices + 1);

                    for (int slice = 0; slice < Slices; slice++)
                    {
                        triangleIndices.Add(top + slice);
                        triangleIndices.Add(bot + slice);
                        triangleIndices.Add(top + slice + 1);
                        triangleIndices.Add(top + slice + 1);
                        triangleIndices.Add(bot + slice);
                        triangleIndices.Add(bot + slice + 1);
                    }
                }

                // RIGHT
                for (int stack = Stacks + 1; stack <= (Stacks * 2); stack++)
                {
                    int top = (stack + 0) * (Slices + 1);
                    int bot = (stack + 1) * (Slices + 1);

                    for (int slice = 0; slice < Slices; slice++)
                    {
                        triangleIndices.Add(top + slice);
                        triangleIndices.Add(bot + slice);
                        triangleIndices.Add(top + slice + 1);
                        triangleIndices.Add(top + slice + 1);
                        triangleIndices.Add(bot + slice);
                        triangleIndices.Add(bot + slice + 1);
                    }
                }

                return triangleIndices;
            }
        }

        public override PointCollection MonoTextureCoordinates
        {
            get
            {
                var textureCoordinates = new PointCollection();

                //Left
                for (int stack = 0; stack <= Stacks; stack++)
                {
                    for (int slice = Slices; slice >= 0; slice--)
                    {
                        textureCoordinates.Add(
                                    new Point((double)slice / Slices,
                                              (double)stack / Stacks));
                    }
                }

                //Right
                for (int stack = 0; stack <= Stacks; stack++)
                {
                    for (int slice = Slices; slice >= 0; slice--)
                    {
                        textureCoordinates.Add(
                                    new Point((double)slice / Slices,
                                              (double)stack / Stacks));
                    }
                }

                return textureCoordinates;
            }
        }

        public override PointCollection OverUnderTextureCoordinates
        {
            get
            {
                var textureCoordinates = new PointCollection();

                //LEFT
                for (int stack = 0; stack <= Stacks; stack++)
                {

                    for (int slice = Slices; slice >= 0; slice--)
                    {
                        textureCoordinates.Add(
                                    new Point((double)slice / Slices,
                                              (double)stack / Stacks / 2));
                    }
                }

                //RIGH
                for (int stack = 0; stack <= Stacks; stack++)
                {
                    for (int slice = Slices; slice >= 0; slice--)
                    {
                        textureCoordinates.Add(new Point(
                            (double)slice / Slices,
                            0.5 + (double)stack / Stacks / 2));
                    }
                }

                return textureCoordinates;
            }
        }

        public override PointCollection SideBySideTextureCoordinates
        {
            get
            {
                var textureCoordinates = new PointCollection();

                //LEFT
                for (int stack = 0; stack <= Stacks; stack++)
                {
                    for (int slice = Slices; slice >= 0; slice--)
                    {
                        textureCoordinates.Add(new Point(
                            (double)slice / Slices / 2,
                            (double)stack / Stacks));
                    }
                }

                //RIGH
                for (int stack = 0; stack <= Stacks; stack++)
                {
                    for (int slice = Slices; slice >= 0; slice--)
                    {
                        textureCoordinates.Add(new Point(
                            0.5 + (double)slice / Slices / 2,
                            (double)stack / Stacks));
                    }
                }

                return textureCoordinates;
            }
        }
    }
}