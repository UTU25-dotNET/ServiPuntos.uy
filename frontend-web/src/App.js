import React, { useState, useEffect } from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import NavBar from "./components/NavBar";
import Home from "./components/Home";
import Login from "./components/Login";
import AuthCallback from "./components/AuthCallback";
import Dashboard from "./components/Dashboard";
import TokenDisplay from "./components/TokenDisplay";
import PrivateRoute from "./components/PrivateRoute";
import DocumentVerification from "./components/DocumentVerification";
import { GoogleOAuthProvider } from "@react-oauth/google";
import apiService from "./services/apiService";

function App() {
  const [tenants, setTenants] = useState([]);          // siempre un array
  const [selectedTenant, setSelectedTenant] = useState(null);

  useEffect(() => {
    apiService
      .getTenants()
      .then(res => {
        const list = res.data;                        // extraemos el array de la respuesta
        setTenants(list);
        // si aún no hay tenant seleccionado, tomamos el primero
        if (list.length && !selectedTenant) {
          setSelectedTenant(list[0]);
        }
      })
      .catch(err => console.error("Error cargando tenants:", err));
  }, []);                                            // solo al montar

  return (
    <GoogleOAuthProvider clientId="533296853256-9t4e46f2t6lpb9ioahpgls7u4aula259.apps.googleusercontent.com">
      <Router>
        <NavBar
          tenants={tenants}
          selectedTenant={selectedTenant}
          onSelectTenant={setSelectedTenant}
        />
        <Routes>
          {/* Rutas públicas */}
          <Route path="/" element={<Home selectedTenant={selectedTenant} />} />
          <Route path="/login" element={<Login />} />
          <Route path="/auth-callback" element={<AuthCallback />} />
          <Route path="/verify-age" element={<DocumentVerification />} />

          {/* Rutas privadas */}
          <Route element={<PrivateRoute />}>
            <Route path="/dashboard" element={<Dashboard />} />
            <Route path="/token" element={<TokenDisplay />} />
          </Route>
        </Routes>
      </Router>
    </GoogleOAuthProvider>
  );
}

export default App;
