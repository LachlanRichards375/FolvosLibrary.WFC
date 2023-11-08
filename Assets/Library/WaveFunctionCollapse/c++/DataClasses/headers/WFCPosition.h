#include <optional>

struct WFCPosition
{
	float x;
	float y;
	std::optional<float> z;
	std::optional<float> w;

	WFCPosition(WFCPosition &other)
	{
		WFCPosition::x = other.x;
		WFCPosition::y = other.y;
		WFCPosition::z = other.z;
		WFCPosition::w = other.w;
	}

	WFCPosition(float x, float y)
	{
		WFCPosition::x = x;
		WFCPosition::y = y;
	}

	WFCPosition(float x, float y, float z)
	{
		WFCPosition::x = x;
		WFCPosition::y = y;
		WFCPosition::z = z;
	}

	WFCPosition(float x, float y, float z, float w)
	{
		WFCPosition::x = x;
		WFCPosition::y = y;
		WFCPosition::z = z;
		WFCPosition::w = w;
	}

	bool operator==(const WFCPosition &other) const
	{
		return (
			this->x == other.x &&
			this->y == other.y &&
			this->z == other.z &&
			this->w == other.w);
	}

	bool operator!=(const WFCPosition &other) const
	{
		return !(
			this->x == other.x &&
			this->y == other.y &&
			this->z == other.z &&
			this->w == other.w);
	}
};
