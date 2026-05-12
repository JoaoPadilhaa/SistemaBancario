import { BrowserRouter, Route, Routes } from 'react-router-dom';
import { Home } from './Componets/home/home';
import { Create } from './Componets/Create/create';
import Logar from "./Pages/Login/logar";
import Registrar from './Pages/Register/register';
import Transacoess from './Pages/Transacoes/transacoes';

function App() {
  return (
    <BrowserRouter>
      <Routes>
          <Route path="/" element={<Home/>} />
          <Route path="/login" element={<Logar/>}/>
          <Route path='/registrar' element={<Registrar/>}/>
          <Route path='/transacoes' element={<Transacoess/>}/>
      </Routes>
    
    </BrowserRouter>
  );
}

export default App;
