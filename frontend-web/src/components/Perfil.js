import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import authService from "../services/authService";
import apiService from "../services/apiService";

const Perfil = () => {
  const [user, setUser] = useState(null);
  const [profileData, setProfileData] = useState({
    // Campos editables
    nombre: "",
    apellido: "",
    email: "",
    telefono: "",
    ciudadResidencia: "",
    combustiblePreferido: "",
    // Campos de contraseña
    currentPassword: "",
    newPassword: "",
    confirmPassword: ""
  });
  const [readOnlyData, setReadOnlyData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [showPasswordChange, setShowPasswordChange] = useState(false);
  const [activeTab, setActiveTab] = useState("personal");
  const navigate = useNavigate();

  // Función para formatear fechas
  const formatDate = (dateString) => {
    if (!dateString || dateString === "0001-01-01T00:00:00") return "No especificado";
    return new Date(dateString).toLocaleDateString();
  };

  // Función para formatear fechas para input
  const formatDateForInput = (dateString) => {
    if (!dateString || dateString === "0001-01-01T00:00:00") return "";
    return dateString.split('T')[0];
  };

  // Cargar datos del usuario al inicializar
  useEffect(() => {
    const loadUserData = async () => {
      try {
        if (!authService.isAuthenticated()) {
          navigate("/login");
          return;
        }

        // Obtener datos básicos del usuario desde el token
        const userData = authService.getCurrentUser();
        setUser(userData);

        // Cargar los datos completos del perfil desde el backend usando email
        const fullProfile = await apiService.getUserProfile();
        
        // Separar datos editables de solo lectura
        setProfileData({
          nombre: fullProfile.nombre || "",
          apellido: fullProfile.apellido || "",
          email: fullProfile.email || "",
          telefono: fullProfile.telefono || "",
          ciudadResidencia: fullProfile.ciudadResidencia || "",
          combustiblePreferido: fullProfile.combustiblePreferido || "",
          currentPassword: "",
          newPassword: "",
          confirmPassword: ""
        });

        setReadOnlyData(fullProfile);

      } catch (err) {
        setError(`Error al cargar los datos del perfil: ${err.message}`);
        
        // Como fallback, usar datos del token
        const userData = authService.getCurrentUser();
        if (userData) {
          setProfileData({
            nombre: userData.uniqueName || userData.name || userData.nombre || "",
            apellido: "",
            email: userData.email || "",
            telefono: "",
            ciudadResidencia: "",
            combustiblePreferido: "",
            currentPassword: "",
            newPassword: "",
            confirmPassword: ""
          });
        }
      } finally {
        setLoading(false);
      }
    };

    loadUserData();
  }, [navigate]);

  // Manejar cambios en los inputs
  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setProfileData(prev => ({
      ...prev,
      [name]: value
    }));
    
    // Limpiar mensajes al editar
    if (error) setError("");
    if (success) setSuccess("");
  };

  const formatCedula = (ci) => {
    if (!ci || ci === 0) return "No especificada";
    
    // Convertir a string y asegurar que tenga 8 dígitos
    const ciString = ci.toString().padStart(8, '0');
    
    // Formato uruguayo: x.xxx.xxx-x
    // Los primeros 7 dígitos son el número, el último es el dígito verificador
    return `${ciString[0]}.${ciString.slice(1, 4)}.${ciString.slice(4, 7)}-${ciString[7]}`;
  };

  // Validar formulario
  const validateForm = () => {
    if (!profileData.nombre.trim()) {
      setError("El nombre es obligatorio");
      return false;
    }

    if (!profileData.email.trim()) {
      setError("El email es obligatorio");
      return false;
    }

    // Validar formato de email
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(profileData.email)) {
      setError("El formato del email no es válido");
      return false;
    }

    // Si se quiere cambiar la contraseña
    if (showPasswordChange) {
      if (!profileData.currentPassword) {
        setError("Ingresa tu contraseña actual para cambiarla");
        return false;
      }

      if (!profileData.newPassword) {
        setError("Ingresa la nueva contraseña");
        return false;
      }

      if (profileData.newPassword.length < 6) {
        setError("La nueva contraseña debe tener al menos 6 caracteres");
        return false;
      }

      if (profileData.newPassword !== profileData.confirmPassword) {
        setError("Las contraseñas nuevas no coinciden");
        return false;
      }
    }

    return true;
  };

  // Enviar formulario
  const handleSubmit = async (e) => {
    e.preventDefault();
    
    if (!validateForm()) return;

    setSaving(true);
    setError("");
    setSuccess("");

    try {
      // Preparar datos para enviar
      const dataToSend = {
        nombre: profileData.nombre.trim(),
        apellido: profileData.apellido.trim(),
        email: profileData.email.trim(),
        telefono: profileData.telefono || 0,
        ciudadResidencia: profileData.ciudadResidencia.trim(),
        combustiblePreferido: profileData.combustiblePreferido.trim()
      };

      // Solo incluir contraseña si se quiere cambiar
      if (showPasswordChange && profileData.newPassword) {
        dataToSend.password = profileData.newPassword;
      }

      // Enviar al backend
      await apiService.updateUserProfile(dataToSend);

      setSuccess("Perfil actualizado exitosamente");
      
      // Limpiar campos de contraseña
      if (showPasswordChange) {
        setProfileData(prev => ({
          ...prev,
          currentPassword: "",
          newPassword: "",
          confirmPassword: ""
        }));
        setShowPasswordChange(false);
      }

      // Recargar datos para reflejar los cambios
      setTimeout(async () => {
        try {
          const updatedProfile = await apiService.getUserProfile();
          setReadOnlyData(updatedProfile);
        } catch (err) {
          console.error("Error al recargar datos:", err);
        }
      }, 1000);

    } catch (err) {
      setError(err.message || "Error al actualizar el perfil");
    } finally {
      setSaving(false);
    }
  };

  // Cancelar edición
  const handleCancel = async () => {
    try {
      // Recargar los datos originales del backend
      const fullProfile = await apiService.getUserProfile();
      setProfileData({
        nombre: fullProfile.nombre || "",
        apellido: fullProfile.apellido || "",
        email: fullProfile.email || "",
        telefono: fullProfile.telefono || "",
        ciudadResidencia: fullProfile.ciudadResidencia || "",
        combustiblePreferido: fullProfile.combustiblePreferido || "",
        currentPassword: "",
        newPassword: "",
        confirmPassword: ""
      });
    } catch (err) {
      // Si falla, usar datos del token como fallback
      const userData = authService.getCurrentUser();
      if (userData) {
        setProfileData({
          nombre: userData.uniqueName || userData.name || userData.nombre || "",
          apellido: "",
          email: userData.email || "",
          telefono: "",
          ciudadResidencia: "",
          combustiblePreferido: "",
          currentPassword: "",
          newPassword: "",
          confirmPassword: ""
        });
      }
    }
    
    setShowPasswordChange(false);
    setError("");
    setSuccess("");
  };

  if (loading) {
    return (
      <div style={{ textAlign: "center", padding: "2rem" }}>
        <div style={{
          border: "4px solid #f3f3f3",
          borderTop: "4px solid #3498db",
          borderRadius: "50%",
          width: "40px",
          height: "40px",
          animation: "spin 2s linear infinite",
          margin: "0 auto 1rem"
        }} />
        <p>Cargando perfil...</p>
        
        <style jsx>{`
          @keyframes spin {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
          }
        `}</style>
      </div>
    );
  }

  return (
    <div style={{ maxWidth: "800px", margin: "0 auto", padding: "1rem" }}>
      <h2>Mi Perfil</h2>

      {/* Mensajes de error y éxito */}
      {error && (
        <div style={{
          backgroundColor: "#f8d7da",
          color: "#721c24",
          padding: "0.75rem",
          borderRadius: "4px",
          marginBottom: "1rem",
          border: "1px solid #f5c6cb"
        }}>
          {error}
        </div>
      )}

      {success && (
        <div style={{
          backgroundColor: "#d4edda",
          color: "#155724",
          padding: "0.75rem",
          borderRadius: "4px",
          marginBottom: "1rem",
          border: "1px solid #c3e6cb"
        }}>
          {success}
        </div>
      )}

      {/* Pestañas de navegación */}
      <div style={{
        display: "flex",
        borderBottom: "2px solid #dee2e6",
        marginBottom: "2rem",
        gap: "1rem"
      }}>
        <button
          onClick={() => setActiveTab("personal")}
          style={{
            padding: "0.75rem 1.5rem",
            border: "none",
            borderBottom: activeTab === "personal" ? "2px solid #007bff" : "2px solid transparent",
            backgroundColor: "transparent",
            color: activeTab === "personal" ? "#007bff" : "#6c757d",
            cursor: "pointer",
            fontWeight: activeTab === "personal" ? "bold" : "normal",
            fontSize: "1rem"
          }}
        >
          📝 Información Personal
        </button>

        <button
          onClick={() => setActiveTab("stats")}
          style={{
            padding: "0.75rem 1.5rem",
            border: "none",
            borderBottom: activeTab === "stats" ? "2px solid #007bff" : "2px solid transparent",
            backgroundColor: "transparent",
            color: activeTab === "stats" ? "#007bff" : "#6c757d",
            cursor: "pointer",
            fontWeight: activeTab === "stats" ? "bold" : "normal",
            fontSize: "1rem"
          }}
        >
          📊 Estadísticas
        </button>
      </div>

      {/* Pestaña de Información Personal */}
      {activeTab === "personal" && (
        <div style={{ display: "grid", gap: "1.5rem" }}>
          
          {/* Tarjeta de Información Editable */}
          <div style={{
            backgroundColor: "#f8f9fa",
            padding: "2rem",
            borderRadius: "12px",
            border: "1px solid #e9ecef",
            boxShadow: "0 2px 4px rgba(0,0,0,0.1)"
          }}>
            <h3 style={{ 
              margin: "0 0 1.5rem 0", 
              color: "#495057",
              display: "flex",
              alignItems: "center",
              gap: "0.5rem",
              fontSize: "1.25rem"
            }}>
              ✏️ Información Personal
            </h3>
            
            <form onSubmit={handleSubmit}>
              <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: "1.25rem", marginBottom: "1.25rem" }}>
                <div>
                  <label htmlFor="nombre" style={{ 
                    display: "block", 
                    marginBottom: "0.5rem", 
                    fontWeight: "600",
                    color: "#495057",
                    fontSize: "0.95rem"
                  }}>
                    👤 Nombre *
                  </label>
                  <input
                    type="text"
                    id="nombre"
                    name="nombre"
                    value={profileData.nombre}
                    onChange={handleInputChange}
                    required
                    style={{
                      width: "100%",
                      padding: "0.75rem",
                      border: "2px solid #e9ecef",
                      borderRadius: "8px",
                      fontSize: "1rem",
                      transition: "border-color 0.3s ease",
                      "&:focus": {
                        borderColor: "#007bff",
                        outline: "none"
                      }
                    }}
                    onFocus={(e) => e.target.style.borderColor = "#007bff"}
                    onBlur={(e) => e.target.style.borderColor = "#e9ecef"}
                  />
                </div>

                <div>
                  <label htmlFor="apellido" style={{ 
                    display: "block", 
                    marginBottom: "0.5rem", 
                    fontWeight: "600",
                    color: "#495057",
                    fontSize: "0.95rem"
                  }}>
                    👥 Apellido
                  </label>
                  <input
                    type="text"
                    id="apellido"
                    name="apellido"
                    value={profileData.apellido}
                    onChange={handleInputChange}
                    style={{
                      width: "100%",
                      padding: "0.75rem",
                      border: "2px solid #e9ecef",
                      borderRadius: "8px",
                      fontSize: "1rem"
                    }}
                    onFocus={(e) => e.target.style.borderColor = "#007bff"}
                    onBlur={(e) => e.target.style.borderColor = "#e9ecef"}
                  />
                </div>
              </div>

              <div style={{ marginBottom: "1.25rem" }}>
                <label htmlFor="email" style={{ 
                  display: "block", 
                  marginBottom: "0.5rem", 
                  fontWeight: "600",
                  color: "#495057",
                  fontSize: "0.95rem"
                }}>
                  📧 Email *
                </label>
                <input
                  type="email"
                  id="email"
                  name="email"
                  value={profileData.email}
                  onChange={handleInputChange}
                  required
                  style={{
                    width: "100%",
                    padding: "0.75rem",
                    border: "2px solid #e9ecef",
                    borderRadius: "8px",
                    fontSize: "1rem"
                  }}
                  onFocus={(e) => e.target.style.borderColor = "#007bff"}
                  onBlur={(e) => e.target.style.borderColor = "#e9ecef"}
                />
              </div>

              <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: "1.25rem", marginBottom: "1.25rem" }}>
                <div>
                  <label htmlFor="telefono" style={{ 
                    display: "block", 
                    marginBottom: "0.5rem", 
                    fontWeight: "600",
                    color: "#495057",
                    fontSize: "0.95rem"
                  }}>
                    📱 Teléfono
                  </label>
                  <input
                    type="tel"
                    id="telefono"
                    name="telefono"
                    value={profileData.telefono}
                    onChange={handleInputChange}
                    placeholder="Ej: 099123456"
                    style={{
                      width: "100%",
                      padding: "0.75rem",
                      border: "2px solid #e9ecef",
                      borderRadius: "8px",
                      fontSize: "1rem"
                    }}
                    onFocus={(e) => e.target.style.borderColor = "#007bff"}
                    onBlur={(e) => e.target.style.borderColor = "#e9ecef"}
                  />
                </div>

                <div>
                  <label htmlFor="ciudadResidencia" style={{ 
                    display: "block", 
                    marginBottom: "0.5rem", 
                    fontWeight: "600",
                    color: "#495057",
                    fontSize: "0.95rem"
                  }}>
                    🏙️ Ciudad de Residencia
                  </label>
                  <input
                    type="text"
                    id="ciudadResidencia"
                    name="ciudadResidencia"
                    value={profileData.ciudadResidencia}
                    onChange={handleInputChange}
                    placeholder="Ej: Montevideo"
                    style={{
                      width: "100%",
                      padding: "0.75rem",
                      border: "2px solid #e9ecef",
                      borderRadius: "8px",
                      fontSize: "1rem"
                    }}
                    onFocus={(e) => e.target.style.borderColor = "#007bff"}
                    onBlur={(e) => e.target.style.borderColor = "#e9ecef"}
                  />
                </div>
              </div>

              <div style={{ marginBottom: "1.5rem" }}>
                <label htmlFor="combustiblePreferido" style={{ 
                  display: "block", 
                  marginBottom: "0.5rem", 
                  fontWeight: "600",
                  color: "#495057",
                  fontSize: "0.95rem"
                }}>
                  ⛽ Combustible Preferido
                </label>
                <select
                  id="combustiblePreferido"
                  name="combustiblePreferido"
                  value={profileData.combustiblePreferido}
                  onChange={handleInputChange}
                  style={{
                    width: "100%",
                    padding: "0.75rem",
                    border: "2px solid #e9ecef",
                    borderRadius: "8px",
                    fontSize: "1rem",
                    backgroundColor: "white"
                  }}
                  onFocus={(e) => e.target.style.borderColor = "#007bff"}
                  onBlur={(e) => e.target.style.borderColor = "#e9ecef"}
                >
                  <option value="">Seleccionar combustible...</option>
                  <option value="Gasolina">🚗 Gasolina</option>
                  <option value="Diesel">🚛 Diesel</option>
                  <option value="Gas">🔥 Gas</option>
                  <option value="Premium">⭐ Premium</option>
                </select>
              </div>

              {/* Botones de acción principales */}
              <div style={{ display: "flex", gap: "0.75rem", flexWrap: "wrap" }}>
                <button
                  type="submit"
                  disabled={saving}
                  style={{
                    backgroundColor: "#28a745",
                    color: "white",
                    border: "none",
                    borderRadius: "8px",
                    padding: "0.75rem 1.5rem",
                    cursor: saving ? "not-allowed" : "pointer",
                    opacity: saving ? 0.7 : 1,
                    fontSize: "1rem",
                    fontWeight: "600",
                    display: "flex",
                    alignItems: "center",
                    gap: "0.5rem",
                    transition: "background-color 0.3s ease"
                  }}
                  onMouseEnter={(e) => !saving && (e.target.style.backgroundColor = "#218838")}
                  onMouseLeave={(e) => !saving && (e.target.style.backgroundColor = "#28a745")}
                >
                  {saving ? "💾 Guardando..." : "💾 Guardar cambios"}
                </button>

                <button
                  type="button"
                  onClick={handleCancel}
                  disabled={saving}
                  style={{
                    backgroundColor: "#6c757d",
                    color: "white",
                    border: "none",
                    borderRadius: "8px",
                    padding: "0.75rem 1.5rem",
                    cursor: "pointer",
                    fontSize: "1rem",
                    fontWeight: "600",
                    display: "flex",
                    alignItems: "center",
                    gap: "0.5rem"
                  }}
                >
                  🔄 Cancelar
                </button>

                <button
                  type="button"
                  onClick={() => navigate("/dashboard")}
                  style={{
                    backgroundColor: "#17a2b8",
                    color: "white",
                    border: "none",
                    borderRadius: "8px",
                    padding: "0.75rem 1.5rem",
                    cursor: "pointer",
                    fontSize: "1rem",
                    fontWeight: "600",
                    display: "flex",
                    alignItems: "center",
                    gap: "0.5rem"
                  }}
                >
                  ← Volver al Dashboard
                </button>
              </div>
            </form>
          </div>

          {/* Tarjeta de Información de Solo Lectura */}
          {readOnlyData && (
            <div style={{ 
              display: "grid",
              gridTemplateColumns: "repeat(auto-fit, minmax(250px, 1fr))",
              gap: "1rem"
            }}>
              
              {/* Información Personal Fija */}
              <div style={{
                backgroundColor: "#e3f2fd",
                padding: "1.5rem",
                borderRadius: "12px",
                border: "1px solid #bbdefb"
              }}>
                <h4 style={{ 
                  margin: "0 0 1rem 0", 
                  color: "#1976d2",
                  display: "flex",
                  alignItems: "center",
                  gap: "0.5rem",
                  fontSize: "1.1rem"
                }}>
                  📋 Información Fija
                </h4>
                <div style={{ display: "grid", gap: "0.75rem" }}>
                  <div>
                    <div style={{ fontSize: "0.85rem", color: "#666", marginBottom: "0.25rem" }}>
                      Fecha de Nacimiento
                    </div>
                    <div style={{ fontWeight: "600", color: "#1976d2" }}>
                      {formatDate(readOnlyData.fechaNacimiento)}
                    </div>
                  </div>
                  
                  {readOnlyData.ci && readOnlyData.ci !== 0 && (
                    <div>
                      <div style={{ fontSize: "0.85rem", color: "#666", marginBottom: "0.25rem" }}>
                        Cédula de Identidad
                      </div>
                      
                      <div style={{ fontWeight: "600", color: "#1976d2" }}>
                        {formatCedula(readOnlyData.ci)}
                      </div>
                    </div>
                  )}
                </div>
              </div>

              {/* Estado de Seguridad */}
              <div style={{
                backgroundColor: readOnlyData.bloqueado ? "#ffebee" : "#e8f5e8",
                padding: "1.5rem",
                borderRadius: "12px",
                border: `1px solid ${readOnlyData.bloqueado ? "#ffcdd2" : "#a5d6a7"}`
              }}>
                <h4 style={{ 
                  margin: "0 0 1rem 0", 
                  color: readOnlyData.bloqueado ? "#d32f2f" : "#388e3c",
                  display: "flex",
                  alignItems: "center",
                  gap: "0.5rem",
                  fontSize: "1.1rem"
                }}>
                  🔒 Estado de Seguridad
                </h4>
                <div style={{ display: "grid", gap: "0.75rem" }}>
                  <div>
                    <div style={{ fontSize: "0.85rem", color: "#666", marginBottom: "0.25rem" }}>
                      Estado de la Cuenta
                    </div>
                    <div style={{ 
                      fontWeight: "600", 
                      color: readOnlyData.bloqueado ? "#d32f2f" : "#388e3c",
                      display: "flex",
                      alignItems: "center",
                      gap: "0.5rem"
                    }}>
                      {readOnlyData.bloqueado ? "🔒 Cuenta Bloqueada" : "✅ Cuenta Activa"}
                    </div>
                  </div>
                  
                  <div>
                    <div style={{ fontSize: "0.85rem", color: "#666", marginBottom: "0.25rem" }}>
                      Intentos Fallidos de Login
                    </div>
                    <div style={{ 
                      fontWeight: "600", 
                      color: readOnlyData.intentosFallidos > 0 ? "#ff9800" : "#388e3c"
                    }}>
                      {readOnlyData.intentosFallidos === 0 ? "Sin intentos fallidos" : `${readOnlyData.intentosFallidos} intentos`}
                    </div>
                  </div>
                </div>
              </div>
            </div>
          )}

          {/* Tarjeta de Cambio de Contraseña */}
          <div style={{
            backgroundColor: "#f8f9fa",
            padding: "2rem",
            borderRadius: "12px",
            border: "1px solid #e9ecef",
            boxShadow: "0 2px 4px rgba(0,0,0,0.1)"
          }}>
            <h4 style={{ 
              margin: "0 0 1rem 0", 
              color: "#495057",
              display: "flex",
              alignItems: "center",
              gap: "0.5rem",
              fontSize: "1.1rem"
            }}>
              🔐 Seguridad de la Cuenta
            </h4>
            
            <button
              type="button"
              onClick={() => setShowPasswordChange(!showPasswordChange)}
              style={{
                backgroundColor: showPasswordChange ? "#6c757d" : "#17a2b8",
                color: "white",
                border: "none",
                borderRadius: "8px",
                padding: "0.75rem 1.25rem",
                cursor: "pointer",
                fontSize: "0.95rem",
                fontWeight: "600",
                display: "flex",
                alignItems: "center",
                gap: "0.5rem",
                transition: "background-color 0.3s ease",
                marginBottom: showPasswordChange ? "1.5rem" : "0"
              }}
            >
              {showPasswordChange ? "❌ Cancelar cambio" : "🔑 Cambiar contraseña"}
            </button>

            {/* Campos de contraseña */}
            {showPasswordChange && (
              <div style={{ 
                padding: "1.5rem",
                backgroundColor: "#fff",
                borderRadius: "8px",
                border: "2px solid #e9ecef"
              }}>
                <div style={{ marginBottom: "1.25rem" }}>
                  <label htmlFor="currentPassword" style={{ 
                    display: "block", 
                    marginBottom: "0.5rem", 
                    fontWeight: "600",
                    color: "#495057",
                    fontSize: "0.95rem"
                  }}>
                    🔒 Contraseña actual *
                  </label>
                  <input
                    type="password"
                    id="currentPassword"
                    name="currentPassword"
                    value={profileData.currentPassword}
                    onChange={handleInputChange}
                    placeholder="Ingresa tu contraseña actual"
                    style={{
                      width: "100%",
                      padding: "0.75rem",
                      border: "2px solid #e9ecef",
                      borderRadius: "8px",
                      fontSize: "1rem"
                    }}
                    onFocus={(e) => e.target.style.borderColor = "#007bff"}
                    onBlur={(e) => e.target.style.borderColor = "#e9ecef"}
                  />
                </div>

                <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: "1.25rem" }}>
                  <div>
                    <label htmlFor="newPassword" style={{ 
                      display: "block", 
                      marginBottom: "0.5rem", 
                      fontWeight: "600",
                      color: "#495057",
                      fontSize: "0.95rem"
                    }}>
                      🆕 Nueva contraseña *
                    </label>
                    <input
                      type="password"
                      id="newPassword"
                      name="newPassword"
                      value={profileData.newPassword}
                      onChange={handleInputChange}
                      placeholder="Mínimo 6 caracteres"
                      style={{
                        width: "100%",
                        padding: "0.75rem",
                        border: "2px solid #e9ecef",
                        borderRadius: "8px",
                        fontSize: "1rem"
                      }}
                      onFocus={(e) => e.target.style.borderColor = "#007bff"}
                      onBlur={(e) => e.target.style.borderColor = "#e9ecef"}
                    />
                    <small style={{ color: "#6c757d", fontSize: "0.8rem", marginTop: "0.25rem", display: "block" }}>
                      La contraseña debe tener al menos 6 caracteres
                    </small>
                  </div>

                  <div>
                    <label htmlFor="confirmPassword" style={{ 
                      display: "block", 
                      marginBottom: "0.5rem", 
                      fontWeight: "600",
                      color: "#495057",
                      fontSize: "0.95rem"
                    }}>
                      ✅ Confirmar contraseña *
                    </label>
                    <input
                      type="password"
                      id="confirmPassword"
                      name="confirmPassword"
                      value={profileData.confirmPassword}
                      onChange={handleInputChange}
                      placeholder="Repite la nueva contraseña"
                      style={{
                        width: "100%",
                        padding: "0.75rem",
                        border: "2px solid #e9ecef",
                        borderRadius: "8px",
                        fontSize: "1rem"
                      }}
                      onFocus={(e) => e.target.style.borderColor = "#007bff"}
                      onBlur={(e) => e.target.style.borderColor = "#e9ecef"}
                    />
                  </div>
                </div>
              </div>
            )}
          </div>
        </div>
      )}

      {/* Pestaña de Estadísticas */}
      {activeTab === "stats" && readOnlyData && (
        <div style={{
          display: "grid",
          gridTemplateColumns: "repeat(auto-fit, minmax(300px, 1fr))",
          gap: "1.5rem"
        }}>
          {/* Tarjeta de Puntos */}
          <div style={{
            backgroundColor: "#e3f2fd",
            padding: "1.5rem",
            borderRadius: "8px",
            border: "1px solid #bbdefb"
          }}>
            <h4 style={{ margin: "0 0 1rem 0", color: "#1976d2", display: "flex", alignItems: "center" }}>
              💰 Puntos y Gastos
            </h4>
            <div style={{ display: "grid", gap: "0.75rem" }}>
              <div style={{ display: "flex", justifyContent: "space-between" }}>
                <strong>Puntos actuales:</strong> 
                <span style={{ color: "#1976d2", fontWeight: "bold" }}>{readOnlyData.puntos}</span>
              </div>
              <div style={{ display: "flex", justifyContent: "space-between" }}>
                <strong>Puntos utilizados:</strong> 
                <span>{readOnlyData.puntosUtilizados}</span>
              </div>
              <div style={{ display: "flex", justifyContent: "space-between" }}>
                <strong>Gasto total:</strong> 
                <span style={{ color: "#28a745", fontWeight: "bold" }}>${readOnlyData.gastoTotal?.toFixed(2) || '0.00'}</span>
              </div>
              <div style={{ display: "flex", justifyContent: "space-between" }}>
                <strong>Gasto promedio:</strong> 
                <span>${readOnlyData.gastoPromedio?.toFixed(2) || '0.00'}</span>
              </div>
            </div>
          </div>

          {/* Tarjeta de Actividad */}
          <div style={{
            backgroundColor: "#f3e5f5",
            padding: "1.5rem",
            borderRadius: "8px",
            border: "1px solid #ce93d8"
          }}>
            <h4 style={{ margin: "0 0 1rem 0", color: "#7b1fa2", display: "flex", alignItems: "center" }}>
              📈 Actividad de Compras
            </h4>
            <div style={{ display: "grid", gap: "0.75rem" }}>
              <div style={{ display: "flex", justifyContent: "space-between" }}>
                <strong>Total de visitas:</strong> 
                <span style={{ color: "#7b1fa2", fontWeight: "bold" }}>{readOnlyData.totalVisitas}</span>
              </div>
              <div style={{ display: "flex", justifyContent: "space-between" }}>
                <strong>Total de compras:</strong> 
                <span style={{ color: "#7b1fa2", fontWeight: "bold" }}>{readOnlyData.totalCompras}</span>
              </div>
              <div style={{ display: "flex", justifyContent: "space-between" }}>
                <strong>Visitas por mes:</strong> 
                <span>{readOnlyData.visitasPorMes?.toFixed(1) || '0.0'}</span>
              </div>
              <div style={{ display: "flex", justifyContent: "space-between" }}>
                <strong>Última visita:</strong> 
                <span style={{ fontSize: "0.9rem" }}>{formatDate(readOnlyData.ultimaVisita)}</span>
              </div>
            </div>
          </div>

          {/* Tarjeta de Información de Cuenta */}
          <div style={{
            backgroundColor: "#e8f5e8",
            padding: "1.5rem",
            borderRadius: "8px",
            border: "1px solid #a5d6a7",
            gridColumn: "1 / -1" // Ocupa todo el ancho disponible
          }}>
            <h4 style={{ margin: "0 0 1rem 0", color: "#388e3c", display: "flex", alignItems: "center" }}>
              📅 Información de Registro
            </h4>
            <div style={{ display: "flex", justifyContent: "center" }}>
              <div style={{ display: "flex", alignItems: "center", gap: "2rem" }}>
                <div style={{ textAlign: "center" }}>
                  <div style={{ fontSize: "0.9rem", color: "#6c757d" }}>Fecha de registro</div>
                  <div style={{ fontSize: "1.1rem", fontWeight: "bold", color: "#388e3c" }}>
                    {formatDate(readOnlyData.fechaCreacion)}
                  </div>
                </div>
                <div style={{ 
                  width: "1px", 
                  height: "40px", 
                  backgroundColor: "#a5d6a7" 
                }}></div>
                <div style={{ textAlign: "center" }}>
                  <div style={{ fontSize: "0.9rem", color: "#6c757d" }}>Días como cliente</div>
                  <div style={{ fontSize: "1.1rem", fontWeight: "bold", color: "#388e3c" }}>
                    {Math.floor((new Date() - new Date(readOnlyData.fechaCreacion)) / (1000 * 60 * 60 * 24))}
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default Perfil;