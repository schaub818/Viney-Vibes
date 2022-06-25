using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VineyVibes
{
    // A game piece consisting of four connected squares.
    public class Tetromino : Piece
    {
        // The amount of time it takes each piece to grow its vine segment.
        public override float GrowTime
        {
            get { return parts[0].GrowTime; }
            set
            {
                foreach (PiecePart part in parts)
                {
                    part.GrowTime = value;
                }
            }
        }

        public override Color SpriteColor 
        {
            get { return parts[0].GetComponent<SpriteRenderer>().color; } 
            set
            {
                foreach (PiecePart part in parts)
                {
                    part.GetComponent<SpriteRenderer>().color = value;
                }
            }
        }

        public override PiecePart EndPart
        {
            get
            {
                if (parts[0].Connected)
                {
                    return parts[parts.Count - 1];
                }
                else
                {
                    return parts[0];
                }
            }
        }

        // The shape of this piece.
        public TetronimoType Type
        {
            get { return type; }
            set { type = value; }
        }

        // The current rotation of this piece.
        public PieceOrientation Orientation
        {
            get { return orientation; }
            set 
            { 
                orientation = value;
                UpdateOrientation();
            }
        }

        // The backing field for the Type property.
        private TetronimoType type;

        // The backing field for the Orientation property.
        private PieceOrientation orientation;

        // Takes in the grid coordinates of the connected part, updates the grid coordinates
        // of all parts, and sets this piece's highest grid Y coordinate.
        public override void Place(int x, int y, int endPointX, int endPointY)
        {
            // Set the local coordinates of all parts
            BuildOffsets();

            if (orientation != PieceOrientation.Vertical && 
                    orientation != PieceOrientation.InvertedVertical)
            {
                RotateOffsets();
            }

            // Use the local coordinates to calculate the grid coordinates of every part and mark
            // the parts as placed.
            //for (int i = 0; i < parts.Count; i++)
            //{
            //    parts[i].X = x + parts[i].OffsetX;
            //    parts[i].Y = y + parts[i].OffsetY;
            //    parts[i].Placed = true;
            //}

            foreach (PiecePart part in parts)
            {
                part.X = x + part.OffsetX;
                part.Y = y + part.OffsetY;
                part.Placed = true;
                part.GetComponent<SpriteRenderer>().sortingLayerName = "Lattice";
            }

            // Set the VineType for each part of this piece.
            SetVinePath(endPointX, endPointY);

            // Loop through each part to determine the highest grid Y coordinate in this piece.
            highestPartY = 0;

            foreach (PiecePart part in parts)
            {
                if (part.Y > highestPartY)
                {
                    highestPartY = part.Y;
                }
            }

            // Mark this piece as placed.
            placed = true;

            // Disable this piece's collider.
            GetComponent<Collider2D>().enabled = false;
        }

        // Remove this piece from the grid.
        public override void Remove()
        {
            // Reset the properties of every part in this piece.
            //for (int i = 0; i < parts.Count; i++)
            //{
            //    parts[i].X = -1;
            //    parts[i].Y = -1;
            //    parts[i].OffsetX = -1;
            //    parts[i].OffsetY = -1;
            //    parts[i].Connected = false;
            //    parts[i].Placed = false;
            //    parts[i].TargetBoxes.Clear();
            //}

            foreach (PiecePart part in parts)
            {
                part.X = -1;
                part.Y = -1;
                part.OffsetX = -1;
                part.OffsetY = -1;
                part.Connected = false;
                part.Placed = false;
                part.TargetBoxes.Clear();
                part.GetComponent<SpriteRenderer>().sortingLayerName = "Unplaced Pieces";
            }

            // Mark this piece as not placed.
            placed = false;

            // Re-enable this piece's collider.
            GetComponent<Collider2D>().enabled = true;
        }

        public override void SetEndPartVineType(int placedX)
        {
            if (parts[0].Connected)
            {
                if (parts[2].Y < parts[3].Y)
                {
                    if (placedX < parts[3].X)
                    {
                        parts[3].vine.Type = VineType.LeftTurn;
                    }
                    else if (placedX > parts[3].X)
                    {
                        parts[3].vine.Type = VineType.RightTurn;
                    }
                    else
                    {
                        parts[3].vine.Type = VineType.Straight;
                    }
                }
                else if (parts[2].X < parts[3].X)
                {
                    if (placedX > parts[3].X)
                    {
                        parts[3].vine.Type = VineType.Straight;
                    }
                    else
                    {
                        parts[3].vine.Type = VineType.LeftTurn;
                    }
                }
                else
                {
                    if (placedX < parts[3].X)
                    {
                        parts[3].vine.Type = VineType.Straight;
                    }
                    else
                    {
                        parts[3].vine.Type = VineType.RightTurn;
                    }
                }
            }
            else
            {
                if (parts[1].Y < parts[0].Y)
                {
                    if (placedX < parts[0].X)
                    {
                        parts[0].vine.Type = VineType.LeftTurn;
                    }
                    else if (placedX > parts[0].X)
                    {
                        parts[0].vine.Type = VineType.RightTurn;
                    }
                    else
                    {
                        parts[0].vine.Type = VineType.Straight;
                    }
                }
                else if (parts[1].X < parts[0].X)
                {
                    if (placedX > parts[0].X)
                    {
                        parts[0].vine.Type = VineType.Straight;
                    }
                    else
                    {
                        parts[0].vine.Type = VineType.LeftTurn;
                    }
                }
                else
                {
                    if (placedX < parts[0].X)
                    {
                        parts[0].vine.Type = VineType.Straight;
                    }
                    else
                    {
                        parts[0].vine.Type = VineType.RightTurn;
                    }
                }
            }
        }

        // Rotates this piece 90 degrees clockwise.
        public void Rotate()
        {
            // Determine the current orientation of the piece, and change it to the next
            // orientation 90 degress clockwise.
            switch (orientation)
            {
                case PieceOrientation.Vertical:
                    orientation = PieceOrientation.Horizontal;

                    break;

                case PieceOrientation.Horizontal:
                    orientation = PieceOrientation.InvertedVertical;

                    break;

                case PieceOrientation.InvertedVertical:
                    orientation = PieceOrientation.InvertedHorizontal;

                    break;

                case PieceOrientation.InvertedHorizontal:
                    orientation = PieceOrientation.Vertical;

                    break;

                default:
                    Debug.LogError("Invalid orientation type:" + orientation.ToString() + " on " + 
                            gameObject.name);

                    return;
            }

            // Rotate the piece based on the new orientation.
            UpdateOrientation();
        }

        public override void Initialize()
        {
            Initialize(TetronimoType.I, PieceOrientation.Vertical);
        }

        public void Initialize(TetronimoType initialType, PieceOrientation initialOrientation)
        {
            // Initialize all fields.
            placed = false;
            growing = false;
            doneGrowing = false;
            highestPartY = 0;
            type = initialType;
            orientation = initialOrientation;

            foreach (PiecePart part in parts)
            {
                part.Initialize();
            }

            vineY = parts[0].Y;

            UpdateOrientation();
        }

        // Grows this piece's vine segment.
        protected override void Grow()
        {
            // If the first part is connected, grow the vine from the first part to the fourth
            // part.
            if (parts[0].Connected)
            {
                // If all parts have finished growing their vine segments, mark that this
                // piece is done growing.
                if (parts[parts.Count - 1].DoneGrowing)
                {
                    growing = false;
                    doneGrowing = true;

                    return;
                }
                    
                // Grow the vine from the first part to the fourth part.
                for (int i = 0; i < parts.Count; i++)
                {
                    if (i == 0 && !parts[i].DoneGrowing && !parts[i].Growing)
                    {
                        parts[i].Growing = true;
                        vineY = parts[i].Y;

                        break;
                    }
                    else if (!parts[i].DoneGrowing && !parts[i].Growing &&
                            parts[i - 1].DoneGrowing)
                    {
                        parts[i].Growing = true;
                        vineY = parts[i].Y;

                        break;
                    }
                }
            }
            // if the fourth part is connected, grow the vine from the fourth part to the first
            // part.
            else
            {
                // If all parts have finished growing their vine segments, mark that this
                // piece is done growing.
                if (parts[0].DoneGrowing)
                {
                    growing = false;
                    doneGrowing = true;

                    return;
                }

                // Grow the vine from the fourth part to the first part.
                for (int i = parts.Count - 1; i > -1; i--)
                {
                    if (i == parts.Count - 1 && !parts[i].DoneGrowing && !parts[i].Growing)
                    {
                        parts[i].Growing = true;
                        vineY = parts[i].Y;

                        break;
                    }
                    else if (!parts[i].DoneGrowing && !parts[i].Growing && 
                            parts[i + 1].DoneGrowing)
                    {
                        parts[i].Growing = true;
                        vineY = parts[i].Y;

                        break;
                    }
                }
            }
        }

        // Update is called once per frame
        private void Update()
        {
            // Grow this piece's vine segment if it is currently growing.
            if (growing)
            {
                Grow();
            }
        }

        // Builds the local coordinates of each part in either the Vertical or InvertedVertical
        // orientation based on which part is connected.
        private void BuildOffsets()
        {
            // Sets which direction to shift the X coordinate of shifted parts.
            int xAdd;
            // Sets which part we should start shifting at as we loop through the parts.
            int shiftPoint;

            // Sets the xAdd and shiftPoint variables based on the piece shape.
            switch (type)
            {
                case TetronimoType.I:
                    xAdd = 0;
                    shiftPoint = 5;

                    break;

                case TetronimoType.J:
                    xAdd = 1;

                    if (parts[0].Connected)
                    {
                        shiftPoint = 1;
                    }
                    else
                    {
                        shiftPoint = 3;
                    }

                    break;

                case TetronimoType.L:
                    xAdd = -1;

                    if (parts[0].Connected)
                    {
                        shiftPoint = 1;
                    }
                    else
                    {
                        shiftPoint = 3;
                    }

                    break;

                case TetronimoType.S:
                    xAdd = -1;
                    shiftPoint = 2;

                    break;

                case TetronimoType.Z:
                    xAdd = 1;
                    shiftPoint = 2;

                    break;

                default:
                    Debug.LogError("Invalid piece type:" + type.ToString() + " on " +
                        gameObject.name);

                    return;
            }

            // If the first part is connected, loop through the parts from first to last.
            if (parts[0].Connected)
            {
                // Loop through each part from the first to the last part and set local
                // coordinates based on the variables set above.
                for (int i = 0; i < 4; i++)
                {
                    // If we have reached or passed the first part to be shifted, shift the local
                    // X coordinate by 1 in the direction of xAdd, and shift the local Y
                    // coordinate down by 1.
                    if (i >= shiftPoint)
                    {
                        parts[i].OffsetX = xAdd;
                        parts[i].OffsetY = i - 1;
                    }
                    // If we have not yet reached the first part to be shifted, set the local
                    // coordinates so each part stacks on top of the previous part.
                    else
                    {
                        parts[i].OffsetX = 0;
                        parts[i].OffsetY = i;
                    }
                }
            }
            // If the last part is connected, loop through the parts from last to first.
            else
            {
                // Loop through each part from the last to the firts part and set local
                // coordinates based on the variables set above.
                for (int i = 3; i > -1; i--)
                {
                    // Stores which iteration of the loop we're on, since the i variable is
                    // counting backward.
                    int iteration = 3 - i;

                    // If we have reached or passed the first part to be shifted, shift the local
                    // X coordinate by 1 in the direction of xAdd, and shift the local Y
                    // coordinate down by 1.
                    if (iteration >= shiftPoint)
                    {
                        parts[i].OffsetX = xAdd;
                        parts[i].OffsetY = iteration - 1;
                    }
                    // If we have not yet reached the first part to be shifted, set the local
                    // coordinates so each part stacks on top of the previous part.
                    else
                    {
                        parts[i].OffsetX = 0;
                        parts[i].OffsetY = iteration;
                    }
                }
            }
        }

        // Rotates the local coordinates of all parts if the piece is in the Horizontal
        // or InvertedHorizontal orientation
        private void RotateOffsets()
        {
            // if the piece is in either vertical orientation, no rotation is needed.
            if (orientation == PieceOrientation.Vertical || 
                    orientation == PieceOrientation.InvertedVertical)
            {
                return;
            }
            
            // Stores the angle of the rotation.
            float angle;
            
            // Stores the current local coordinates of the part whose coordinates are being
            // rotated.
            int currentX;
            int currentY;

            // Set the rotation angle based on the piece shape.
            if (type == TetronimoType.J || type == TetronimoType.Z)
            {
                angle = Mathf.Deg2Rad * 90.0f;
            }
            else if (type == TetronimoType.L || type == TetronimoType.S)
            {
                angle = Mathf.Deg2Rad * 270.0f;
            }
            // If this is a straight piece, set the rotation angle based on which part is
            // connected.
            else
            {
                if (orientation == PieceOrientation.Horizontal)
                {
                    if (parts[0].Connected)
                    {
                        angle = Mathf.Deg2Rad * 270.0f;
                    }
                    else
                    {
                        angle = Mathf.Deg2Rad * 90.0f;
                    }
                }
                else
                {
                    if (parts[0].Connected)
                    {
                        angle = Mathf.Deg2Rad * 90.0f;
                    }
                    else
                    {
                        angle = Mathf.Deg2Rad * 270.0f;
                    }
                }
                
            }

            // Loop through each part and rotate its local coordinates.
            for (int i = 0; i < 4; i++)
            {
                // Store the local coordinates of the part currently being looped through.
                currentX = parts[i].OffsetX;
                currentY = parts[i].OffsetY;

                // Rotate the part's local coordinates by the given angle.
                parts[i].OffsetX = (currentX * (int)Mathf.Cos(angle)) - 
                        (currentY * (int)Mathf.Sin(angle));
                parts[i].OffsetY = (currentY * (int)Mathf.Cos(angle)) + 
                        (currentX * (int)Mathf.Sin(angle));
            }
        }

        // Sets the shape of the vine segment that each part of this piece should grow.
        private void SetVinePath(int endPointX, int endPointY)
        {
            int previousX = endPointX;
            int previousY = endPointY;
            int nextX;
            int type;
            int typeModifier = 0;

            if (parts[0].Connected)
            {
                for (int i = 0; i < parts.Count; i++)
                {
                    if (i == parts.Count - 1)
                    {
                        typeModifier = 1;
                        nextX = parts[i].X;
                    }
                    else
                    {
                        nextX = parts[i + 1].X;
                    }

                    if (previousY < parts[i].Y)
                    {
                        if (nextX < parts[i].X)
                        {
                            type = (int)VineType.LeftTurn + typeModifier;
                        }
                        else if (nextX > parts[i].X)
                        {
                            type = (int)VineType.RightTurn + typeModifier;
                        }
                        else
                        {
                            type = (int)VineType.Straight + typeModifier;
                        }
                    }
                    else if (previousX < parts[i].X)
                    {
                        parts[i].vine.transform.Rotate(0.0f, 0.0f, 270.0f);

                        if (nextX > parts[i].X)
                        {
                            type = (int)VineType.Straight + typeModifier;
                        }
                        else
                        {
                            type = (int)VineType.LeftTurn + typeModifier;
                        }
                    }
                    else
                    {
                        parts[i].vine.transform.Rotate(0.0f, 0.0f, 90.0f);

                        if (nextX < parts[i].X)
                        {
                            type = (int)VineType.Straight + typeModifier;
                        }
                        else
                        {
                            type = (int)VineType.RightTurn + typeModifier;
                        }
                    }

                    parts[i].vine.Type = (VineType)type;

                    previousX = parts[i].X;
                    previousY = parts[i].Y;
                }
            }
            else
            {
                for (int i = parts.Count - 1; i > -1; i--)
                {
                    if (i == 0)
                    {
                        typeModifier = 1;
                        nextX = parts[i].X;
                    }
                    else
                    {
                        nextX = parts[i - 1].X;
                    }

                    if (previousY < parts[i].Y)
                    {
                        if (nextX < parts[i].X)
                        {
                            type = (int)VineType.LeftTurn + typeModifier;
                        }
                        else if (nextX > parts[i].X)
                        {
                            type = (int)VineType.RightTurn + typeModifier;
                        }
                        else
                        {
                            type = (int)VineType.Straight + typeModifier;
                        }
                    }
                    else if (previousX < parts[i].X)
                    {
                        parts[i].vine.transform.Rotate(0.0f, 0.0f, 270.0f);

                        if (nextX > parts[i].X)
                        {
                            type = (int)VineType.Straight + typeModifier;
                        }
                        else
                        {
                            type = (int)VineType.LeftTurn + typeModifier;
                        }
                    }
                    else
                    {
                        parts[i].vine.transform.Rotate(0.0f, 0.0f, 90.0f);

                        if (nextX < parts[i].X)
                        {
                            type = (int)VineType.Straight + typeModifier;
                        }
                        else
                        {
                            type = (int)VineType.RightTurn + typeModifier;
                        }
                    }

                    parts[i].vine.Type = (VineType)type;

                    previousX = parts[i].X;
                    previousY = parts[i].Y;
                }
            }
        }

        // Changes this piece's rotation to the given orientation.
        private void UpdateOrientation()
        {
            // Stores the angle of the rotation.
            float angle;

            // Set the rotation angle and which parts are connectable based on the current
            // orientation and piece type.
            switch (orientation)
            {
                case PieceOrientation.Vertical:
                    angle = 0.0f;

                    parts[0].Connectable = true;
                    parts[3].Connectable = false;

                    break;

                case PieceOrientation.Horizontal:
                    angle = 270.0f;

                    if (type == TetronimoType.L || type == TetronimoType.S)
                    {
                        parts[0].Connectable = true;
                        parts[3].Connectable = false;
                    }
                    else if (type == TetronimoType.J || type == TetronimoType.Z)
                    {
                        parts[0].Connectable = false;
                        parts[3].Connectable = true;
                    }
                    else
                    {
                        parts[0].Connectable = true;
                        parts[3].Connectable = true;
                    }

                    break;

                case PieceOrientation.InvertedVertical:
                    angle = 180.0f;

                    parts[0].Connectable = false;
                    parts[3].Connectable = true;

                    break;

                case PieceOrientation.InvertedHorizontal:
                    angle = 90.0f;

                    if (type == TetronimoType.L || type == TetronimoType.S)
                    {
                        parts[0].Connectable = false;
                        parts[3].Connectable = true;
                    }
                    else if (type == TetronimoType.J || type == TetronimoType.Z)
                    {
                        parts[0].Connectable = true;
                        parts[3].Connectable = false;
                    }
                    else
                    {
                        parts[0].Connectable = true;
                        parts[3].Connectable = true;
                    }

                    break;

                default:
                    Debug.LogError("Invalid orientation type:" + orientation.ToString() + " on " +
                        gameObject.name);

                    return;
            }

            // Rotate the piece's game object on the Z axis by the rotation determined above.
            transform.rotation = Quaternion.identity;
            transform.Rotate(0.0f, 0.0f, angle);

            // Reset the rotation of each part's vine back to default.
            foreach (PiecePart part in parts)
            {
                part.vine.transform.rotation = Quaternion.identity;
            }
        }
    }
}