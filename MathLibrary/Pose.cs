namespace ZippsterStudios.Math
{
    public class Pose
    {
        private Coordinate Coord { get; set; }
        private Quaternion Orientation { get; set; }

        public Pose(Coordinate coord, Quaternion orientation)
        {
            Coord = coord;
            Orientation = orientation;
        }

        public void LookAt(Vector3 target)
        {
            Orientation = Quaternion.LookAt(Coord.Ecef.Ecef, target, CalculateWorldUp());
        }

        #region OrientationVectors
        public Vector3 Forward => RotateVectorByQuaternion(new Vector3(0, 0, 1), Orientation);
        public Vector3 Up => RotateVectorByQuaternion(new Vector3(0, 1, 0), Orientation);
        public Vector3 Right => RotateVectorByQuaternion(new Vector3(1, 0, 0), Orientation);
        public Vector3 Left => RotateVectorByQuaternion(new Vector3(-1, 0, 0), Orientation);
        public Vector3 Down => RotateVectorByQuaternion(new Vector3(0, -1, 0), Orientation);

        private Vector3 CalculateWorldUp()
        {
            Vector3 position = Coord.Ecef.Ecef;
            return position.Normalize();
        }

        private Vector3 RotateVectorByQuaternion(Vector3 vector, Quaternion quaternion)
        {
            Quaternion qVector = new Quaternion(0, vector.X, vector.Y, vector.Z);
            Quaternion qConjugate = quaternion.Conjugate();
            Quaternion qResult = quaternion * qVector * qConjugate;
            return new Vector3(qResult.X, qResult.Y, qResult.Z);
        }
        #endregion
    }
}
