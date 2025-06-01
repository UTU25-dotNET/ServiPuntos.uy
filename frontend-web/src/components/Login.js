import React, { useState, useEffect } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import authService from "../services/authService";
import '../App.css';

const Login = () => {
    const [credentials, setCredentials] = useState({ email: "", password: "" });
    const [registerData, setRegisterData] = useState({ 
        name: "", 
        email: "", 
        password: "", 
        confirmPassword: "",
        ci: "",
        tenantId: "" // **NUEVO: Campo para tenant**
    });
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");
    const [urlError, setUrlError] = useState("");
    const [emailFromUrl, setEmailFromUrl] = useState("");
    const [isRegisterMode, setIsRegisterMode] = useState(false);
    const [tenants, setTenants] = useState([]); // **NUEVO: Lista de tenants**
    const [loadingTenants, setLoadingTenants] = useState(false); // **NUEVO: Loading para tenants**
    const navigate = useNavigate();
    const [searchParams] = useSearchParams();

    useEffect(() => {
        if (authService.isAuthenticated()) {
            navigate("/dashboard");
        }

        // Leer parámetros de la URL
        const errorParam = searchParams.get('error');
        const emailParam = searchParams.get('email');

        if (errorParam) {
            setUrlError(decodeURIComponent(errorParam));
            
            if (emailParam) {
                setEmailFromUrl(decodeURIComponent(emailParam));
                setCredentials(prev => ({
                    ...prev,
                    email: decodeURIComponent(emailParam)
                }));
                setRegisterData(prev => ({
                    ...prev,
                    email: decodeURIComponent(emailParam)
                }));
            }
        }
    }, [navigate, searchParams]);

    // **NUEVO: Cargar tenants cuando se cambia a modo registro**
    useEffect(() => {
        console.log("useEffect ejecutándose - isRegisterMode:", isRegisterMode, "tenants.length:", tenants.length);
        
        if (isRegisterMode && tenants.length === 0) {
            console.log("Llamando a loadTenants...");
            loadTenants();
        }
    }, [isRegisterMode]);

    // Ponemos esto para cargar los tenants en el registro
    const loadTenants = async () => {
        try {
            setLoadingTenants(true);
            console.log("Cargando tenants desde el servicio...");
            
            const tenantsData = await authService.getTenants();
            console.log("Tenants cargados:", tenantsData);
            
            setTenants(tenantsData);
        } catch (err) {
            console.error("Error al cargar tenants:", err);
            setError("Error al cargar la lista de empresas: " + (err.message || "Error desconocido"));
        } finally {
            setLoadingTenants(false);
        }
    };

    const handleChange = (e) => {
        const { name, value } = e.target;
        if (isRegisterMode) {
            setRegisterData((prev) => ({
                ...prev,
                [name]: value,
            }));
        } else {
            setCredentials((prev) => ({
                ...prev,
                [name]: value,
            }));
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError("");
        setUrlError("");

        try {
            if (isRegisterMode) {
                // Validar que las contraseñas coincidan
                if (registerData.password !== registerData.confirmPassword) {
                    setError("Las contraseñas no coinciden");
                    return;
                }

                // Validar que todos los campos estén llenos (incluyendo CI y TenantId)
                if (!registerData.name || !registerData.email || !registerData.password || 
                    !registerData.ci || !registerData.tenantId) {
                    setError("Todos los campos son obligatorios");
                    return;
                }

                // Validar formato de CI
                const ciRegex = /^\d{7,8}$/;
                if (!ciRegex.test(registerData.ci)) {
                    setError("La Cédula de Identidad debe tener entre 7 y 8 dígitos");
                    return;
                }

                // Llamar al servicio de registro con tenantId
                await authService.register(
                    registerData.name, 
                    registerData.email, 
                    registerData.password,
                    registerData.ci,
                    registerData.tenantId // **NUEVO: Incluir tenantId**
                );
                
                // Si el registro es exitoso, cambiar a modo login
                setIsRegisterMode(false);
                setCredentials({ email: registerData.email, password: "" });
                setError("");
                alert("Registro exitoso. Ahora puedes iniciar sesión.");
                
            } else {
                // Login normal
                await authService.login(credentials.email, credentials.password);
                window.location.href = "/dashboard";
            }
        } catch (err) {
            setError(err.message || (isRegisterMode ? "Error en el registro" : "Error en el inicio de sesión"));
        } finally {
            setLoading(false);
        }
    };

    const handleGoogleLogin = async () => {
        window.location.href = "https://localhost:5019/api/auth/google-login";
    };

    const applyTestAccount = (type) => {
        setError("");
        setUrlError("");
        setIsRegisterMode(false);
        
        if (type === "user") {
            setCredentials({ email: "user@gmail.com", password: "user" });
        } else if (type === "admin") {
            setCredentials({ email: "admin@gmail.com", password: "admin" });
        }
    };

    const toggleMode = () => {
        setIsRegisterMode(!isRegisterMode);
        setError("");
        setUrlError("");
        setCredentials({ email: "", password: "" });
        setRegisterData({ name: "", email: "", password: "", confirmPassword: "", ci: "", tenantId: "" });
    };

    return (
        <div className="login-container">
            <div className="login-image">
                <img src="/imagenDeLogin.jpg" alt="Estación de servicio" />
            </div>

            <div className="login-form">
                <h2 className="title">Servipuntos</h2>
                <h3>{isRegisterMode ? "Registro" : "Iniciar Sesión"}</h3>
                <p>¡La app donde tus compras sí rinden!</p>

                {/* Error de autenticación normal */}
                {error && <div className="error-message">{error}</div>}
                
                {/* Error desde URL */}
                {urlError && (
                    <div className="url-error-message" style={{
                        backgroundColor: "#fff3cd",
                        border: "1px solid #ffeaa7",
                        color: "#856404",
                        padding: "12px",
                        borderRadius: "6px",
                        marginBottom: "1rem",
                        textAlign: "center",
                        fontSize: "14px"
                    }}>
                        <strong>⚠️ {urlError}</strong>
                        {emailFromUrl && (
                            <div style={{ marginTop: "8px", fontSize: "12px" }}>
                                Email detectado: <em>{emailFromUrl}</em>
                            </div>
                        )}
                    </div>
                )}

                <form onSubmit={handleSubmit}>
                    {isRegisterMode && (
                        <>
                                <div className="input-group">
                                <span className="icon">🏢</span>
                                <select
                                    name="tenantId"
                                    value={registerData.tenantId}
                                    onChange={handleChange}
                                    required
                                    disabled={loadingTenants}
                                    style={{
                                        width: "100%",
                                        padding: "0.75rem",
                                        border: "1px solid #ddd",
                                        borderRadius: "4px",
                                        fontSize: "16px",
                                        backgroundColor: loadingTenants ? "#f5f5f5" : "white"
                                    }}
                                >
                                    <option value="">
                                        {loadingTenants ? "Cargando empresas..." : "Elija una gasolinera"}
                                    </option>
                                    {tenants.map(tenant => (
                                        <option key={tenant.id} value={tenant.id}>
                                            {tenant.nombre}
                                        </option>
                                    ))}
                                </select>
                            </div>

                            <div className="input-group">
                                <span className="icon">👤</span>
                                <input
                                    type="text"
                                    name="name"
                                    value={registerData.name}
                                    onChange={handleChange}
                                    placeholder="Nombre completo"
                                    required
                                />
                            </div>

                            <div className="input-group">
                                <span className="icon">🆔</span>
                                <input
                                    type="text"
                                    name="ci"
                                    value={registerData.ci}
                                    onChange={handleChange}
                                    placeholder="Cédula de Identidad (ej: 12345678)"
                                    pattern="[0-9]{7,8}"
                                    title="La CI debe tener entre 7 y 8 dígitos"
                                    maxLength="8"
                                    required
                                />
                            </div>
                        </>
                    )}

                    <div className="input-group">
                        <span className="icon">📧</span>
                        <input
                            type="email"
                            name="email"
                            value={isRegisterMode ? registerData.email : credentials.email}
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
                            value={isRegisterMode ? registerData.password : credentials.password}
                            onChange={handleChange}
                            placeholder="********"
                            required
                        />
                    </div>

                    {isRegisterMode && (
                        <div className="input-group">
                            <span className="icon">🔒</span>
                            <input
                                type="password"
                                name="confirmPassword"
                                value={registerData.confirmPassword}
                                onChange={handleChange}
                                placeholder="Confirmar contraseña"
                                required
                            />
                        </div>
                    )}

                    <button className="login-btn" type="submit" disabled={loading || loadingTenants}>
                        {loading ? (isRegisterMode ? "Registrando..." : "Iniciando sesión...") : (isRegisterMode ? "Registrarse" : "Iniciar Sesión")}
                    </button>

                    {/* Botón para alternar entre login y registro */}
                    <div style={{ textAlign: "center", marginTop: "1rem" }}>
                        <button 
                            type="button"
                            onClick={toggleMode}
                            style={{
                                background: "none",
                                border: "none",
                                color: "#007bff",
                                textDecoration: "underline",
                                cursor: "pointer",
                                fontSize: "14px"
                            }}
                        >
                            {isRegisterMode ? "¿Ya tienes cuenta? Inicia sesión aquí" : "¿No tienes cuenta? Regístrate aquí"}
                        </button>
                    </div>

                    {!isRegisterMode && (
                        <div style={{ marginTop: "2rem", textAlign: "center" }}>
                            <div style={{ marginBottom: "1rem" }}>
                                <button
                                    type="button"
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
                    )}
                </form>

                {!isRegisterMode && (
                    <div className="test-accounts">
                        <h4>Cuentas de prueba</h4>
                        <div className="test-buttons">
                            <button onClick={() => applyTestAccount("user")}>Usuario</button>
                            <button onClick={() => applyTestAccount("admin")}>Admin</button>
                        </div>
                    </div>
                )}
            </div>
        </div>
    );
}

export default Login;