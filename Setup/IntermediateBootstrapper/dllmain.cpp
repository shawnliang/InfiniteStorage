#include "stdafx.h"
#include <Bootstrapper.h>
#include <Prerequisites.h>

using namespace SharpSetup;
using namespace std;

class SetupBootstrapper : public BootstrapperBase
{
public:
	virtual DWORD OnUnpackingComplete()
	{
		PrerequisiteManager pm(*this);
		if(!isInstalledDotNet(L"4.0"))
			installDotNet(&pm, L"4.0");

		pm.getFiles();
		pm.performInstall();

		return S_OK;
	}
};
CREATEBOOTSTRAPPER(SetupBootstrapper);
