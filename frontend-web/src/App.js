import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import NavBar from "./components/NavBar";
import Home from "./components/Home";
import Login from "./components/Login";
import AuthCallback from './components/AuthCallback';
import Dashboard from "./components/Dashboard";
import Perfil from "./components/Perfil";
import TokenDisplay from "./components/TokenDisplay";
import Estaciones from "./components/Estaciones"; // Nuevo componente
import PrivateRoute from "./components/PrivateRoute";
import DocumentVerification from "./components/DocumentVerification";
import { GoogleOAuthProvider } from "@react-oauth/google";

function App() {
  return (
    <GoogleOAuthProvider clientId="533296853256-9t4e46f2t6lpb9ioahpgls7u4aula259.apps.googleusercontent.com">
      <Router>
        <NavBar />
        <Routes>
          {/* Rutas públicas */}
          <Route path="/" element={<Home />} />
          <Route path="/login" element={<Login />} />
          <Route path="/auth-callback" element={<AuthCallback />} />
          <Route path="/verify-age" element={<DocumentVerification />} />

          {/* Rutas privadas */}
          <Route element={<PrivateRoute />}>
            <Route path="/dashboard" element={<Dashboard />} />
            <Route path="/perfil" element={<Perfil />} />
            <Route path="/token" element={<TokenDisplay />} />
            <Route path="/estaciones" element={<Estaciones />} /> {/* Nueva ruta para el listado de estaciones */}
          </Route>
        </Routes>
      </Router>
    </GoogleOAuthProvider>
  );
}

export default App;