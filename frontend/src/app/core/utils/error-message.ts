// 10-11-2026 Construido por: Ing GAAE - Translates HTTP and ProblemDetails errors into user messages.

interface ProblemDetailsBody {
  detail?: string;
  title?: string;
  errors?: Record<string, string[]>;
}

interface HttpErrorLike {
  status?: number;
  error?: ProblemDetailsBody;
  message?: string;
}

export function extractErrorMessage(error: unknown): string {
  const httpError = error as HttpErrorLike | null;

  if (httpError?.status === 0) {
    return 'No se pudo conectar con el servidor. Verifica que la API esté en ejecución.';
  }

  const detail = httpError?.error?.detail;
  if (detail) {
    return detail;
  }

  const firstValidationError = Object.values(httpError?.error?.errors ?? {})
    .flat()
    .find((message) => !!message);
  if (firstValidationError) {
    return firstValidationError;
  }

  const title = httpError?.error?.title;
  if (title) {
    return title;
  }

  return 'Ocurrió un error inesperado.';
}
