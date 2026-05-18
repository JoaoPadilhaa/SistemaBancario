import { BrowserRouter, Route, Routes } from 'react-router-dom';
import { Home } from './Componets/home/home';
import Logar from "./Pages/Login/logar";
import Registrar from './Pages/Register/register';
import Transacoess from './Pages/Transacoes/transacoes';
import Pix from './Pages/Pix/pix';
import ProtectRoute from './Componets/ProtectedRoute/protectedroute';
import Perfil from './Pages/Perfil/perfil';

function App() {
  return (
    <BrowserRouter>
      <Routes>
          <Route path="/" element={<ProtectRoute><Home/></ProtectRoute>} />
          <Route path="/login" element={<Logar/>}/>
          <Route path='/registrar' element={<Registrar/>}/>
          <Route path='/transacoes' element={<ProtectRoute><Transacoess/></ProtectRoute>}/>
          <Route path='/pix' element={<ProtectRoute><Pix/></ProtectRoute>}/>
          <Route path='/perfil' element={<ProtectRoute><Perfil/></ProtectRoute>}></Route>
      </Routes>
    
    </BrowserRouter>
  );
}

export default App;
