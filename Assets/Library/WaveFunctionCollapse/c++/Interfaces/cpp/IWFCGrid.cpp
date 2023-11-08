#include "IWFCGrid.h"

void IWFCGrid ::SetSize(WFCPosition size)
{
	WFC2DGrid::size = size;
}

WFCPosition IWFCGrid ::GetSize()
{
	return size;
}