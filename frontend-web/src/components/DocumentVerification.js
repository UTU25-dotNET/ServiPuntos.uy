import React, { useState, useEffect } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import authService from "../services/authService";

// Renombra el componente a DocumentVerification para mantener consistencia con el nombre del archivo
const DocumentVerification = () => {
    const [cedula, setCedula] = useState("");
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");
    const [userData, setUserData] = useState(null);
    const [debugInfo, setDebugInfo] = useState(""); // Para mostrar informaci�n de depuraci�n
    const location = useLocation();
    const navigate = useNavigate();

    // Extraer token y returnUrl de los query params
    const params = new URLSearchParams(location.search);
    const token = params.get("token");
    const code = params.get("code");
    const state = params.get("state");
    const returnUrl = params.get("returnUrl") || "/auth-callback";

    useEffect(() => {
        // A�adir informaci�n de depuraci�n
        console.log("Token recibido:", token);
        console.log("Code:", code);
        console.log("State:", state);

        // Si no hay token, redirigir al login
        if (!token) {
            console.error("No se recibi� token en los par�metros");
            setError("No se pudo obtener la informaci�n de autenticaci�n. Int�ntalo de nuevo.");
            setTimeout(() => navigate("/login"), 3000);
            return;
        }

        // Decodificar el token para mostrar datos del usuario
        try {
            // Verificar si authService.decodeToken existe
            if (typeof authService.decodeToken !== 'function') {
                throw new Error("La funci�n decodeToken no est� disponible");
            }

            const decoded = authService.decodeToken(token);
            console.log("Token decodificado:", decoded);

            if (!decoded) {
                throw new Error("No se pudo decodificar el token");
            }

            // Extraer la informaci�n del usuario del token
            const userData = decoded.payload || decoded;
            setUserData(userData);

            // Mostrar informaci�n de debug
            setDebugInfo(`Token recibido: ${token.substring(0, 20)}...`);

        } catch (err) {
            console.error("Error al decodificar token:", err);
            // En lugar de redirigir inmediatamente, mostramos el error y permitimos continuar
            setError(`Error al procesar la informaci�n de usuario: ${err.message}`);

            // Establecer un userData b�sico para permitir que el formulario se muestre
            setUserData({ name: "usuario" });
        }
    }, [token, code, state, navigate]);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError("");

        if (!cedula.trim()) {
            setError("Por favor ingresa tu c�dula");
            setLoading(false);
            return;
        }

        try {
            // Construir la URL con todos los par�metros necesarios
            let callbackUrl = `https://localhost:5019/api/auth/google-callback?cedula=${encodeURIComponent(cedula)}`;

            // A�adir code y state si est�n disponibles
            if (code) callbackUrl += `&code=${encodeURIComponent(code)}`;
            if (state) callbackUrl += `&state=${encodeURIComponent(state)}`;

            console.log("Redirigiendo a:", callbackUrl);

            // Redirigir al callback de Google con la c�dula
            window.location.href = callbackUrl;
        } catch (err) {
            setError(err.message || "Error al verificar la edad");
            setLoading(false);
        }
    };

    // Mostrar el formulario incluso si no tenemos datos de usuario, pero con menos personalizaci�n
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