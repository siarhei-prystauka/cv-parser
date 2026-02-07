import './App.css'
import { Navigate, Route, Routes } from 'react-router-dom'
import { AdminLayout } from './layouts/AdminLayout'
import { MainLayout } from './layouts/MainLayout'
import { AdminPage } from './pages/AdminPage'
import { ProfilesPage } from './pages/ProfilesPage'

const App = () => (
  <Routes>
    <Route element={<MainLayout />}>
      <Route path="/" element={<ProfilesPage />} />
    </Route>
    <Route element={<AdminLayout />}>
      <Route path="/admin" element={<AdminPage />} />
    </Route>
    <Route path="*" element={<Navigate to="/" replace />} />
  </Routes>
)

export default App
