using Microsoft.AspNetCore.Components;

namespace Jennifer.Wasm.Admin.Common;

public interface ILoginOnlyPage { }

public class LoginPageBase : ComponentBase, ILoginOnlyPage { }
