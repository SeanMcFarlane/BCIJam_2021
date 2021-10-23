// Shapes © Freya Holmér - https://twitter.com/FreyaHolmer/
// Website & Documentation - https://acegikmo.com/shapes/

namespace Shapes {

	public enum DashType {
		Basic,
		Angled,
		Rounded
	}

	public static class DashTypeExtensions {
		public static bool HasModifier( this DashType type ) {
			switch( type ) {
				case DashType.Angled: return true;
				default:              return false;
			}
		}
	}

}