import { extractErrorMessage } from './error-message';

// 10-11-2026 Construido por: Ing GAAE - Unit tests for the shared API error translator.
describe('extractErrorMessage', () => {
  it('should return the ProblemDetails detail', () => {
    const error = { status: 422, error: { detail: 'No se puede pasar de Pendiente a Completada.' } };

    expect(extractErrorMessage(error)).toBe('No se puede pasar de Pendiente a Completada.');
  });

  it('should return the first validation error when there is no detail', () => {
    const error = { status: 400, error: { errors: { Description: ['La descripción es obligatoria.'] } } };

    expect(extractErrorMessage(error)).toBe('La descripción es obligatoria.');
  });

  it('should return a friendly message when the API is unreachable', () => {
    const error = { status: 0, message: 'Http failure response for http://localhost:5080/api/users: 0 Unknown Error' };

    expect(extractErrorMessage(error)).toBe(
      'No se pudo conectar con el servidor. Verifica que la API esté en ejecución.',
    );
  });

  it('should return the generic fallback for unknown errors', () => {
    expect(extractErrorMessage(null)).toBe('Ocurrió un error inesperado.');
  });
});
