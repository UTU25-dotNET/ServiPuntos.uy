import React, { useEffect } from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import NavBar from "./components/layout/NavBar";
import Home from "./components/Home";
import Login from "./components/auth/Login";
import AuthCallback from "./components/auth/AuthCallback";
import Perfil from "./components/perfil/Perfil";
import TokenDisplay from "./components/TokenDisplay";
import EstacionesList from "./components/estaciones/EstacionesList";
import PrivateRoute from "./components/PrivateRoute";
import DocumentVerification from "./components/auth/DocumentVerification";
import PayPalResult from "./components/PayPalResult";
import { GoogleOAuthProvider } from "@react-oauth/google";
import authService from "./services/authService";

function App() {
  useEffect(() => {
    authService.scheduleAutoLogout();
  }, []);

  return (
    <GoogleOAuthProvider clientId="533296853256-9t4e46f2t6lpb9ioahpgls7u4aula259.apps.googleusercontent.com">
      <Router>
        <NavBar />
        <Routes>
          {/* Rutas públicas */}
          <Route path="/" element={<Home />} />
          <Route path="/login" element={<Login />} />
          <Route path="/auth-callback" element={<AuthCallback />} />
          <Route path="/paypal-return" element={<PayPalResult />} />
          <Route path="/paypal-cancel" element={<PayPalResult />} />
          <Route path="/verify-age" element={<DocumentVerification />} />

          {/* Rutas privadas */}
          <Route element={<PrivateRoute />}>
            <Route path="/perfil" element={<Perfil />} />
            <Route path="/token" element={<TokenDisplay />} />
            <Route path="/estaciones" element={<EstacionesList />} />
          </Route>
        </Routes>
      </Router>
    </GoogleOAuthProvider>
  );
}

export default App;