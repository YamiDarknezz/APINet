namespace APINet.Models
{
    /// <summary>
    /// Wrapper de respuesta JSend estándar
    /// Especificación: https://github.com/omniti-labs/jsend
    /// </summary>
    public class JSendResponse<T>
    {
        /// <summary>
        /// Estado de la respuesta: "success", "fail", "error"
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Datos de la respuesta (para success y fail)
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Mensaje descriptivo (solo para error)
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Código de error adicional (opcional para error)
        /// </summary>
        public int? Code { get; set; }

        /// <summary>
        /// Crea una respuesta exitosa (200-299)
        /// </summary>
        public static JSendResponse<T> Success(T data)
        {
            return new JSendResponse<T>
            {
                Status = "success",
                Data = data
            };
        }

        /// <summary>
        /// Crea una respuesta de fallo de validación (400, 422)
        /// </summary>
        public static JSendResponse<T> Fail(T data)
        {
            return new JSendResponse<T>
            {
                Status = "fail",
                Data = data
            };
        }

        /// <summary>
        /// Crea una respuesta de error del servidor (500+)
        /// </summary>
        public static JSendResponse<object> Error(string message, int? code = null)
        {
            return new JSendResponse<object>
            {
                Status = "error",
                Message = message,
                Code = code
            };
        }
    }

    /// <summary>
    /// Wrapper sin tipo genérico para casos simples
    /// </summary>
    public class JSendResponse : JSendResponse<object>
    {
        public static JSendResponse Success()
        {
            return new JSendResponse
            {
                Status = "success",
                Data = null
            };
        }

        public static new JSendResponse Fail(object data)
        {
            return new JSendResponse
            {
                Status = "fail",
                Data = data
            };
        }

        public static new JSendResponse Error(string message, int? code = null)
        {
            return new JSendResponse
            {
                Status = "error",
                Message = message,
                Code = code
            };
        }
    }
}
