using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;
using System.Runtime.ConstrainedExecution;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WrapperMercadoPagoAPI.Model;
internal class SucursalStates
{
    public List<string> States = new()
    {
        "Buenos Aires","Capital Federal","Catamarca","Chaco","Chubut","Corrientes","Córdoba","Entre Ríos","Formosa","Jujuy","La Pampa","La Rioja","Mendoza","Misiones","Neuquén","Río Negro","Salta","San Juan","San Luis","Santa Cruz","Santa Fe","Santiago del Estero","Tierra del Fuego","Tucumán"
    };
    public Dictionary<string, List<string>>  Statements = new Dictionary<string, List<string>>
    {
        ["Capital Federal"] = new List<string>()
        { 
            "Agronomía", "Almagro", "Balvanera", "Barracas", "Barrio Norte", "Belgrano", "Belgrano Barrancas", "Belgrano C", "Belgrano Chico", "Belgrano R", "Boedo", "Botánico",
            "Caballito", "Chacarita", "Coghlan", "Colegiales", "Constitución", "Flores", "Floresta", "La Boca", "Las Cañitas", "Liniers", "Mataderos", "Monserrat", "Monte Castro",
            "Nueva Pompeya", "Núñez", "Palermo", "Palermo Chico", "Palermo Hollywood", "Palermo Nuevo", "Palermo Soho", "Palermo Viejo", "Parque Avellaneda", "Parque Chacabuco",
            "Parque Chas", "Parque Patricios", "Paternal", "Puerto Madero", "Recoleta", "Retiro", "Saavedra", "San Cristóbal", "San Nicolás", "San Telmo", "Santa Rita", "Velez Sarsfield",
            "Versailles", "Villa Crespo", "Villa Devoto", "Villa Gral. Mitre", "Villa Lugano", "Villa Luro", "Villa Ortúzar", "Villa Pueyrredón", "Villa Real", "Villa Riachuelo",
            "Villa Soldati", "Villa Urquiza", "Villa del Parque" 
        },
        ["Buenos Aires"] = new List<string>()
        {
            "12 De Octubre","25 de Mayo","4 DE noviembre","9 de Julio","Adela","Adolfo Alsina","Adolfo Gonzales Chaves","Aeropuerto Ezeiza","Aguara","Aguas Verdes","Alagon",
            "Alberti","Alegre","Alejandro Korn","Almirante Brown","Alsina","Alto Los Cardales","Amalia","Araujo","Arrecifes","Arroyo Aleli","Arroyo Pareja","Arturo Segui",
            "Arturo Vatteone","Atucha","Avellaneda","Ayacucho","Azul","Bahía Blanca","Balcarce","Baradero","Base Aeronaval Cmte Espora","Base Aeronaval Punta Indio",
            "Base Naval Rio Santiago","Baterias","Beccar","Benito Juárez","Berazategui","Berisso","Beruti","Blaquier","Bolívar","Boulogne","Bragado","Brandsen","Cabildo",
            "Calfucura","Campana","Capitán Sarmiento","Cariló","Carlos Casares","Carlos Tejedor","Carmen de Areco","Caseros","Castelar","Castelli","Cañada La Rica","Cañuelas",
            "Centro Agricola El Pato","Chacabuco","Chapadmalal","Chascomús","Chivilcoy","Ciudad Jardin Del Palomar","Coliqueo","Colman","Colón","Comandante Giribone","Cooper",
            "Coronel Dorrego","Coronel Granada","Coronel Pringles","Coronel Rosales","Coronel Suárez","Costa Chica","Costa Esmeralda","Costa del Este","Cuartel Iv","Cucha Cucha",
            "Daireaux","Del Viso","Dolores","Don Torcuato","Dos Hermanos","Drabble","El Durazno","El Espinillo","El Perdido Est Jose Guisasola","El Pino","El Socorro","El Trio",
            "Emilio Lamarca","Ensenada","Esc Nav Militar Rio Sant","Escobar","Espartillar","Estacion Lago Epecuen","Estacion Moreno","Estancia Las Gamas","Estancias",
            "Esteban De Luca","Esteban Echeverría","Exaltación de la Cruz","Ezeiza","Flamenco","Florencio Varela","Florentino Ameghino","Fortin Irene","Fortin Lavalle","Garro",
            "General Alvarado","General Alvear","General Arenales","General Belgrano","General Daniel Cerri","General Guido","General La Madrid","General Las Heras",
            "General Lavalle","General Madariaga","General Paz","General Pinto","General Pueyrredón","General Rodríguez","General San Martín","General Viamonte","General Villegas",
            "Guaminí","Hipólito Yrigoyen","Hurlingham","Ingeniero Allan","Ingeniero Budge","Ingeniero De Madrid","Ingeniero Williams","Ireneo Portela","Isla Los Laureles",
            "Isla Martin Garcia","Ituzaingó","Jose Clemente Paz","Jose Ferrari","Jose Leon Suarez","José C. Paz","Juancho","Junín","La Caleta","La Colina","La Dorita",
            "La Esperanza (ROSAS","PDO.LAS FLORES)","La Lucila del Mar","La Matanza","La Pastora","La Plata","La Querencia","La Viticola","Lanús","Laprida","Las Flores",
            "Las Toninas","Leandro N. Alem","Leubuco","Lincoln","Lobería","Lobos","Lomas de Zamora","Los Cardales","Luján","Magdalena","Maipú","Malvinas Argentinas",
            "Manuel Alberti","Manuel B Gonnet","Mar Azul","Mar Chiquita","Mar De Cobo","Mar de Ajo","Mar de las Pampas","Mar del Plata","Mar del Sur","Mar del Tuyu",
            "Marcos Paz","Masurel","Mecha","Mercedes","Merlo","Miramar","Monte","Monte Hermoso","Moreno","Morón","Navarro","Necochea","Olavarría","Ostende","Palantelen",
            "Patagones","Pehuajó","Pellegrini","Pergamino","Pila","Pilar","Pinamar","Presidente Perón","Primera Junta","Punta Indio","Punta Lara","Punta Médanos","Puán",
            "Quilmes","Quiñihual Estacion","Ramallo","Ranchos","Rauch","Remedios De Escalada","Rio Tala","Rivadavia","Rojas","Roque Pérez","Saavedra","Saladillo","Salliqueló",
            "Salto","San Andres","San Andrés de Giles","San Antonio de Areco","San Bernardo","San Bernardo Del Tuyu","San Cayetano","San Clemente","San Clemente Del Tuyu",
            "San Fernando","San Isidro","San Miguel","San Nicolás","San Pedro","San Vicente","Santa Clara del Mar","Santa Teresita","Suipacha","Tandil","Tapalqué","Tigre",
            "Tordillo","Tornquist","Trenque Lauquen","Tres Arroyos","Tres Lomas","Tres de febrero","Valeria del Mar","Verónica","Vicente López","Villa Adelina","Villa Gesell",
            "Villa Numancia","Villa Robles","Villarino","Wilde","Zona Delta San Fernando","Zona Delta Tigre","Zárate"
        }
    };
}

