import React, { useState, useEffect } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import authService from "../services/authService";

const DocumentVerification = () => {
    const [cedula, setCedula] = useState("");
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");
    const [userData, setUserData] = useState(null);
    const [debugInfo, setDebugInfo] = useState("");
    const location = useLocation();
    const navigate = useNavigate();

    // Extraer token y returnUrl de los query params
    const params = new URLSearchParams(location.search);
    const token = params.get("token");
    const code = params.get("code");
    const state = params.get("state");
    const returnUrl = params.get("returnUrl") || "/auth-callback";

    useEffect(() => {
        // Lógica existente...
        console.log("Token recibido:", token);
        console.log("Code:", code);
        console.log("State:", state);

        if (!token) {
            console.error("No se recibió token en los parámetros");
            setError("No se pudo obtener la información de autenticación. Inténtalo de nuevo.");
            setTimeout(() => navigate("/login"), 3000);
            return;
        }

        try {
            if (typeof authService.decodeToken !== 'function') {
                throw new Error("La función decodeToken no está disponible");
            }

            const decoded = authService.decodeToken(token);
            console.log("Token decodificado:", decoded);

            if (!decoded) {
                throw new Error("No se pudo decodificar el token");
            }

            const userData = decoded.payload || decoded;
            setUserData(userData);
            setDebugInfo(`Token recibido: ${token.substring(0, 20)}...`);

        } catch (err) {
            console.error("Error al decodificar token:", err);
            setError(`Error al procesar la información de usuario: ${err.message}`);
            setUserData({ name: "usuario" });
        }
    }, [token, code, state, navigate]);

    // Nueva función para formatear la cédula automáticamente
    const formatCedula = (value) => {
        // Eliminar todos los caracteres no numéricos
        const numbers = value.replace(/\D/g, '');
        
        // Si no hay números, devolver cadena vacía
        if (numbers.length === 0) return '';
        
        // Formatear según la cantidad de dígitos
        if (numbers.length <= 1) {
            return numbers;
        } else if (numbers.length <= 4) {
            return `${numbers.slice(0, 1)}.${numbers.slice(1)}`;
        } else if (numbers.length <= 7) {
            return `${numbers.slice(0, 1)}.${numbers.slice(1, 4)}.${numbers.slice(4)}`;
        } else {
            return `${numbers.slice(0, 1)}.${numbers.slice(1, 4)}.${numbers.slice(4, 7)}-${numbers.slice(7, 8)}`;
        }
    };

    // Manejador para el cambio en el input
    const handleCedulaChange = (e) => {
        const inputValue = e.target.value;
        const formattedValue = formatCedula(inputValue);
        setCedula(formattedValue);
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError("");

        if (!cedula.trim()) {
            setError("Por favor ingresa tu cédula");
            setLoading(false);
            return;
        }

        try {
            let callbackUrl = `https://localhost:5019/api/auth/google-callback?cedula=${encodeURIComponent(cedula)}`;

            if (code) callbackUrl += `&code=${encodeURIComponent(code)}`;
            if (state) callbackUrl += `&state=${encodeURIComponent(state)}`;

            console.log("Redirigiendo a:", callbackUrl);
            window.location.href = callbackUrl;
        } catch (err) {
            setError(err.message || "Error al verificar la edad");
            setLoading(false);
        }
    };

    return (
        <div style={{ maxWidth: "400px", margin: "0 auto", padding: "1rem" }}>
            <h2 style={{ color: "#7B3F00" }}>Servipuntos.uy</h2>
            <h3>Verificacion de identidad</h3>

            {userData && (
                <div style={{ marginBottom: "1rem" }}>
                    <p>Hola {userData.name || "usuario"}!</p>
                    <p>Para continuar con el proceso de registro, necesitamos verificar tu identidad.</p>
                </div>
            )}

            {!userData && (
                <div style={{ marginBottom: "1rem" }}>
                    <p>Para continuar con el proceso de registro, necesitamos verificar tu identidad.</p>
                </div>
            )}

            {error && (
                <div
                    style={{
                        backgroundColor: "#f8d7da",
                        color: "#721c24",
                        padding: "0.75rem",
                        borderRadius: "4px",
                        marginBottom: "1rem",
                    }}
                >
                    {error}
                </div>
            )}

            <form onSubmit={handleSubmit}>
                <div style={{ marginBottom: "1rem" }}>
                    <label
                        htmlFor="cedula"
                        style={{ display: "block", marginBottom: "0.5rem" }}
                    >
                        Cedula de identidad:
                    </label>
                    <input
                        type="text"
                        id="cedula"
                        value={cedula}
                        onChange={handleCedulaChange}
                        placeholder="X.XXX.XXX-X"
                        required
                        style={{
                            width: "100%",
                            padding: "0.5rem",
                            borderRadius: "4px",
                            border: "1px solid #ced4da",
                        }}
                        maxLength="11" // Longitud máxima: 8 dígitos + 3 separadores
                    />
                    <small style={{ color: "#6c757d" }}>
                        Ingresa solo los números, los separadores se agregarán automáticamente
                    </small>
                </div>

                <button
                    type="submit"
                    disabled={loading}
                    style={{
                        backgroundColor: "#007bff",
                        color: "white",
                        border: "none",
                        borderRadius: "4px",
                        padding: "0.5rem 1rem",
                        cursor: loading ? "not-allowed" : "pointer",
                        opacity: loading ? 0.7 : 1,
                    }}
                >
                    {loading ? "Verificando..." : "Verificar identidad"}
                </button>
            </form>
        </div>
    );
};

export default DocumentVerification;