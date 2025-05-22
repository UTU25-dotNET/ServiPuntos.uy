import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import authService from "../services/authService";
import { GoogleLogin } from "@react-oauth/google";
import '../App.css';

const Login = () => {
    const [credentials, setCredentials] = useState({ email: "", password: "" });
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");
    const navigate = useNavigate();

    useEffect(() => {
        if (authService.isAuthenticated()) {
            navigate("/dashboard");
        }
    }, [navigate]);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setCredentials((prev) => ({
            ...prev,
            [name]: value,
        }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError("");

        try {
            await authService.login(credentials.email, credentials.password);
            window.location.href = "/dashboard";
        } catch (err) {
            setError(err.message || "Error en el inicio de sesión");
        } finally {
            setLoading(false);
        }
    };

    const handleGoogleLogin = async (credentialResponse) => {
        window.location.href = "https://localhost:5019/api/auth/google-login";
    };

    const applyTestAccount = (type) => {
        if (type === "user") {
            setCredentials({ email: "user@gmail.com", password: "user" });
        } else if (type === "admin") {
            setCredentials({ email: "admin@gmail.com", password: "admin" });
        }
    };

    return (
        <div className="login-container">
            <div className="login-image">
                <img src="/imagenDeLogin.jpg"
                     alt="Estación de servicio" />
            </div>

            <div className="login-form">
                <h2 className="title">Servipuntos</h2>
                <h3>Iniciar Sesión</h3>
                <p>¡La app donde tus compras sí rinden!</p>

                {error && <div className="error-message">{error}</div>}

                <form onSubmit={handleSubmit}>
                    <div className="input-group">
                        <span className="icon">👤</span>
                        <input
                            type="email"
                            name="email"
                            value={credentials.email}
                            onChange={handleChange}
                            placeholder="Email@gmail.com"
                            required
                        />
                    </div>

                    <div className="input-group">
                        <span className="icon">🔒</span>
                        <input
                            type="password"
                            name="password"
                            value={credentials.password}
                            onChange={handleChange}
                            placeholder="********"
                            required
                        />
                    </div>

                    <button className="login-btn" type="submit" disabled={loading}>
                        {loading ? "Iniciando sesión..." : "Iniciar Sesión"}
                    </button>

                    <div style={{ marginTop: "2rem", textAlign: "center" }}>
                        <div style={{ marginBottom: "1rem" }}>
                            <button
                            onClick={handleGoogleLogin}
                                style={{
                                    backgroundColor: "#4285F4",
                                    color: "white",
                                    border: "none",
                                    borderRadius: "4px",
                                    padding: "0.75rem 1rem",
                                    cursor: "pointer"
                                }}
                            >
                            Iniciar sesión con Google
                             </button>
                        </div>
                    </div>
                </form>

                <div className="test-accounts">
                    <h4>Cuentas de prueba</h4>
                    <div className="test-buttons">
                        <button onClick={() => applyTestAccount("user")}>Usuario</button>
                        <button onClick={() => applyTestAccount("admin")}>Admin</button>
                    </div>
                </div>
            </div>
        </div>
    );
}
    export default Login;
    