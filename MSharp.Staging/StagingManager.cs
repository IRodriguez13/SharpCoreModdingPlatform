namespace MSharp.ModLoader.StagingSystem;

// -- Aca manejo el staging de los payloads en base a las respuestas del adapter --

public class StagingManager<T>
{
	private readonly Stack<T> _history = new(); // El registro de cambios (en RAM)
	private T? _current; // Estado actual del payload
	private readonly Action<T> _applyCallback; //
	private readonly Action<T> _rollbackCallback;

	public StagingManager(Action<T> applyCallback, Action<T> rollbackCallback)
	{
		_applyCallback = applyCallback;
		_rollbackCallback = rollbackCallback;
	}

	public void MSadd(T next)
	{
		if (_current != null) _history.Push(_current); _current = next;

		try
		{
			_applyCallback(_current); // Aplicamos el cambio actual
			Console.WriteLine("[Staging] Instruccion aplicada");
		}
		catch (Exception)
		{
			MSrevert(); // Automático por si la aplicación falla
			Console.WriteLine("[Staging] Error al aplicar la instrucción. Se ha revertido el cambio.");
			_current = default;
			_history.Clear();
			return;
		}
	}

	public void MSrevert()
	{
		if (_history.Count == 0)
		{
			Console.WriteLine("[Staging] No hay versiones previas para hacer rollback.");
			return;
		}

		// Rollbackear sería limpiar la pila y volver al último estado válido
		Console.WriteLine("[Staging] Revirtiendo al último estado válido");
		_current = _history.Pop();
		_rollbackCallback(_current);
		Console.WriteLine("[Staging] Rollback exitoso");
		Console.WriteLine($"[Staging] Estado actual: {_current}");
	}

 // Confirmamos estado actual como final. Aca el commit es simplemente dejar current como está y limpiar la pila.
	public void MScommit() => _history.Clear();

// Devuelve el estado actual, o null si no hay ninguno - es como una memoria ram
	public T? MSgetCurrent() => _current; 
}
