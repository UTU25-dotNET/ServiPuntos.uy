import React, { useState, useEffect } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import authService from "../services/authService";

// Renombra el componente a DocumentVerification para mantener consistencia con el nombre del archivo
const DocumentVerification = () => {
    const [cedula, setCedula] = useState("");
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");
    const [userData, setUserData] = useState(null);
    const [debugInfo, setDebugInfo] = useState(""); // Para mostrar información de depuración
    const location = useLocation();
    const navigate = useNavigate();

    // Extraer token y returnUrl de los query params
    const params = new URLSearchParams(location.search);
    const token = params.get("token");
    const code = params.get("code");
    const state = params.get("state");
    const returnUrl = params.get("returnUrl") || "/auth-callback";

    useEffect(() => {
        // Añadir información de depuración
        console.log("Token recibido:", token);
        console.log("Code:", code);
        console.log("State:", state);

        // Si no hay token, redirigir al login
        if (!token) {
            console.error("No se recibió token en los parámetros");
            setError("No se pudo obtener la información de autenticación. Inténtalo de nuevo.");
            setTimeout(() => navigate("/login"), 3000);
            return;
        }

        // Decodificar el token para mostrar datos del usuario
        try {
            // Verificar si authService.decodeToken existe
            if (typeof authService.decodeToken !== 'function') {
                throw new Error("La función decodeToken no está disponible");
            }

            const decoded = authService.decodeToken(token);
            console.log("Token decodificado:", decoded);

            if (!decoded) {
                throw new Error("No se pudo decodificar el token");
            }

            // Extraer la información del usuario del token
            const userData = decoded.payload || decoded;
            setUserData(userData);

            // Mostrar información de debug
            setDebugInfo(`Token recibido: ${token.substring(0, 20)}...`);

        } catch (err) {
            console.error("Error al decodificar token:", err);
            // En lugar de redirigir inmediatamente, mostramos el error y permitimos continuar
            setError(`Error al procesar la información de usuario: ${err.message}`);

            // Establecer un userData básico para permitir que el formulario se muestre
            setUserData({ name: "usuario" });
        }
    }, [token, code, state, navigate]);

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
            // Construir la URL con todos los parámetros necesarios
            let callbackUrl = `https://localhost:5019/api/auth/google-callback?cedula=${encodeURIComponent(cedula)}`;

            // Añadir code y state si están disponibles
            if (code) callbackUrl += `&code=${encodeURIComponent(code)}`;
            if (state) callbackUrl += `&state=${encodeURIComponent(state)}`;

            console.log("Redirigiendo a:", callbackUrl);

            // Redirigir al callback de Google con la cédula
            window.location.href = callbackUrl;
        } catch (err) {
            setError(err.message || "Error al verificar la edad");
            setLoading(false);
        }
    };

    // Mostrar el formulario incluso si no tenemos datos de usuario, pero con menos personalización
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
                        onChange={(e) => setCedula(e.target.value)}
                        placeholder="X.XXX.XXX-X"
                        required
                        style={{
                            width: "100%",
                            padding: "0.5rem",
                            borderRadius: "4px",
                            border: "1px solid #ced4da",
                        }}
                    />
                    <small style={{ color: "#6c757d" }}>
                        Formato: X.XXX.XXX-X (con puntos y guion)
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
