using UnityEngine;

namespace FolvosLibrary.WFC
{
	public struct IWFCPosition
	{
		public float x;
		public float y;
		public float? z;
		public float? w;

		public IWFCPosition(IWFCPosition other)
		{
			this.x = other.x;
			this.y = other.y;
			this.z = other.z;
			this.w = other.w;
		}

		public IWFCPosition(float x, float y)
		{
			this.x = x;
			this.y = y;
			this.z = null;
			this.w = null;
		}

		public IWFCPosition(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = null;
		}

		public IWFCPosition(float x, float y, float z, float w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public IWFCPosition(Vector2 size)
		{
			this.x = size.x;
			this.y = size.y;
			this.z = null;
			this.w = null;
		}

		public IWFCPosition(Vector3 size)
		{
			this.x = size.x;
			this.y = size.y;
			this.z = size.z;
			this.w = null;
		}

		public IWFCPosition(Vector4 size)
		{
			this.x = size.x;
			this.y = size.y;
			this.z = size.z;
			this.w = size.w;
		}

		public Vector2 AsVector2()
		{
			return new Vector2(x, y);
		}
		public Vector2Int AsVector2Int()
		{
			return new Vector2Int(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
		}

		public Vector3 AsVector3()
		{
			if (z is null)
			{
				return Vector3.zero;
			}
			return new Vector3(x, y, z.Value);
		}
		public Vector3Int AsVector3Int()
		{
			if (z is null)
			{
				return Vector3Int.zero;
			}
			return new Vector3Int(Mathf.RoundToInt(x), Mathf.RoundToInt(y), Mathf.RoundToInt(z.Value));
		}

		public bool IsVector3()
		{
			return !(z is null);
		}

		public Vector4 AsVector4()
		{
			if (z is null || w is null)
			{
				return Vector4.zero;
			}

			return new Vector4(Mathf.RoundToInt(x), Mathf.RoundToInt(y), Mathf.RoundToInt(z.Value), Mathf.RoundToInt(w.Value));
		}

		public bool IsVector4()
		{
			return !(z is null && w is null);
		}

		public override string ToString()
		{
			string s = $"({x},{y}";

			if (IsVector3())
			{
				s += $",{z}";
			}

			if (IsVector4())
			{
				s += $",{w}";
			}

			return s + ")";
		}

		public static bool operator ==(IWFCPosition pos1, IWFCPosition pos2)
		{
			return (
				pos1.x == pos2.x &&
				pos1.y == pos2.y &&
				pos1.z == pos2.z &&
				pos1.w == pos2.w
			);
		}

		public static bool operator !=(IWFCPosition pos1, IWFCPosition pos2)
		{
			return !(
				pos1.x == pos2.x &&
				pos1.y == pos2.y &&
				pos1.z == pos2.z &&
				pos1.w == pos2.w
			);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !this.GetType().Equals(obj.GetType()))
			{
				return false;
			}
			else
			{
				IWFCPosition other = (IWFCPosition)obj;
				return this == other;
			}
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}