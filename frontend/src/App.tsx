import { BrowserRouter, Route, Routes } from 'react-router-dom';
import { Home } from './Componets/home/home';
import Transacoess from './Pages/Transacoes/transacoes';
import Pix from './Pages/Pix/pix';
import ProtectRoute from './Componets/ProtectedRoute/protectedroute';
import Perfil from './Pages/Perfil/perfil';
import { UserProvider } from './UserContext';
import Cadastro from './Pages/CompletarCadastro/cadastro';

function App() {
  return (
    <UserProvider>
      <BrowserRouter>
        <Routes>
            <Route path="/" element={<ProtectRoute><Home/></ProtectRoute>} />
            <Route path='/transacoes' element={<ProtectRoute><Transacoess/></ProtectRoute>}/>
            <Route path='/pix' element={<ProtectRoute><Pix/></ProtectRoute>}/>
            <Route path='/perfil' element={<ProtectRoute><Perfil/></ProtectRoute>}></Route>
            <Route path='/completar-cadastro' element={<Cadastro/>}></Route>
        </Routes>
      
      </BrowserRouter>
    </UserProvider>
  );
}

export default App;
