import React from 'react'
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom'
import { GoogleOAuthProvider } from '@react-oauth/google'
import { TenantProvider } from './context/TenantContext'
import NavBar from './components/NavBar'
import Home from './components/Home'
import Login from './components/Login'
import AuthCallback from './components/AuthCallback'
import DocumentVerification from './components/DocumentVerification'
import Dashboard from './components/Dashboard'
import TokenDisplay from './components/TokenDisplay'
import PrivateRoute from './components/PrivateRoute'

function App() {
  return (
    <GoogleOAuthProvider clientId="533296853256-9t4e46f2t6lpb9ioahpgls7u4aula259.apps.googleusercontent.com">
      <TenantProvider>
        <Router>
          <NavBar />
          <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/login" element={<Login />} />
            <Route path="/auth-callback" element={<AuthCallback />} />
            <Route path="/verify-age" element={<DocumentVerification />} />
            <Route element={<PrivateRoute />}>
              <Route path="/dashboard" element={<Dashboard />} />
              <Route path="/token" element={<TokenDisplay />} />
            </Route>
          </Routes>
        </Router>
      </TenantProvider>
    </GoogleOAuthProvider>
  )
}

export default App
