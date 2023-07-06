// Array de productos
const productos = {
    producto1: {
        nombre: 'Papas fritas',
        precio: '3.50',
        descripcion: 'Papas frescas preparadas en el momento, caseras, seleccionadas exclusivamente para tener una buena calidad y con un sabor inigualable.',
        srcImg: 'https://i.blogs.es/f9cf25/degustacion-patatas/450_1000.jpg'
    },
    producto2: {
        nombre: 'Hamburguesa',
        precio: '10.00',
        descripcion: 'La mejor Hamburguesa del mercado, con productos finamente seleccionados, una presentación única y el mejor sabor.',
        srcImg: 'https://cocina-casera.com/wp-content/uploads/2016/11/hamburguesa-queso-receta.jpg'
    },
    producto3: {
        nombre: 'Pizza',
        precio: '15.50',
        descripcion: 'Masa preparada en el local, fermentada el tiempo suficiente para que de una mordida puedas sentir una sensación única de sabor y textura.',
        srcImg: 'https://elgourmet.s3.amazonaws.com/recetas/share/pizza_Mh3H4eanyBKEsStv1YclPWTf9OUqIi.png'
    },
    producto4: {
        nombre: 'Completo',
        precio: '8.50',
        descripcion: 'Estos son los mejores completos de la ciudad, hechos con el mejor pan y ingredientes finamente seleccionados.',
        srcImg: 'https://i2.wp.com/golososdelmundo.com/wp-content/uploads/2018/08/completo-italiano3.jpg?fit=1024%2C683'
    }
}
// Se captura el template de los productos
const templateProd = document.getElementById('template-prod').content
const contenedorProd = document.querySelector('.contenedor-productos')
const fragment = document.createDocumentFragment()


// TODO LO RELACIONADO A AGREGAR LOS PRODUCTOS AL DOM
Object.values(productos).forEach(producto => {
    templateProd.querySelector('.div-info .nombre-prod').textContent = producto.nombre
    templateProd.querySelector('.div-precio-boton .precio').textContent = producto.precio
    templateProd.querySelector('.div-info .descripcion-prod').textContent = producto.descripcion
    templateProd.querySelector('.contenedor-img img').setAttribute('alt', producto.nombre)
    templateProd.querySelector('.contenedor-img img').setAttribute('src', producto.srcImg)
    const clone = templateProd.cloneNode(true)
    fragment.appendChild(clone)
})
contenedorProd.appendChild(fragment)

// TODO LO RELACIONADO AL CARRITO DE COMPRA
let carrito = {}
const templateTabla = document.getElementById('agregar-producto-al-carro').content
const tbodyCarrito = document.getElementById('carrito-body')
const fragmentTabla = document.createDocumentFragment()
const templateFoot = document.getElementById('tfooter').content
const tfootCarrito = document.getElementById('footer')

contenedorProd.addEventListener('click', e => {

    if (e.target.textContent === "Agregar") {
        setCarrito(e.target.parentElement.parentElement)
    }
    e.stopPropagation();
})

const setCarrito = e => {
    const pivoteCarrito = {
        nombre: e.querySelector('.div-info .nombre-prod').textContent,
        precio: e.querySelector('.div-precio-boton .precio').textContent,
        cantidad: 1
    }
    if (carrito.hasOwnProperty(pivoteCarrito.nombre)) {
        carrito[pivoteCarrito.nombre].cantidad += 1
    } else {
        carrito[pivoteCarrito.nombre] = { ...pivoteCarrito }
    }
    pintarTabla(carrito)
}

const pintarTabla = objetoCarrito => {
    Object.values(objetoCarrito).forEach(objeto => {
        const cloneTabla = templateTabla.cloneNode(true)
        cloneTabla.getElementById('producto').textContent = objeto.nombre
        cloneTabla.getElementById('cant').textContent = objeto.cantidad
        cloneTabla.getElementById('precio-uni').textContent = objeto.precio
        let precioTotal = parseFloat(objeto.precio) * objeto.cantidad
        cloneTabla.getElementById('precio-total-prod').textContent = precioTotal.toFixed(2)
        fragmentTabla.appendChild(cloneTabla)
    })
    tbodyCarrito.innerHTML = ''
    tbodyCarrito.appendChild(fragmentTabla)
    pintarFooter()
}

const pintarFooter = () => {
    tfootCarrito.innerHTML = ''
    if (Object.keys(carrito).length === 0) {
        tfootCarrito.innerHTML = '<tr><td colspan = 4>¡No hay ningun elemento en el carrito!</td></tr>'
    } else {
        const total = Object.values(carrito).reduce((acc, { cantidad, precio }) => acc + (cantidad * precio), 0)
        templateFoot.getElementById('total-a-pagar').textContent = total.toFixed(2)
        const cloneFoot = templateFoot.cloneNode(true)
        fragment.appendChild(cloneFoot)
        tfootCarrito.appendChild(fragment)
        //Boton Vaciar carrito
        const botonVaciar = document.getElementById('vaciar-tabla')
        botonVaciar.addEventListener('click', () => {
            carrito = {}
            pintarTabla(carrito)
            pintarFooter()
        })

        //Botones aumentar y disminuir cantidades

    }
}
tbodyCarrito.addEventListener('click', e => {

    if (e.target.classList.contains('button')) {
        aumentarDisminuir(e.target)
    }
})
const aumentarDisminuir = boton => {
    if (boton.textContent === '+') {
        const indicador = boton.parentElement.parentElement.firstElementChild.textContent
        Object.values(carrito).forEach(elemento => {
            if (elemento.nombre === indicador) {
                carrito[elemento.nombre].cantidad++
            }
        })
    }
    if (boton.textContent === '-') {
        const indicador = boton.parentElement.parentElement.firstElementChild.textContent
        Object.values(carrito).forEach(elemento => {
            if (elemento.nombre === indicador) {
                carrito[elemento.nombre].cantidad--
                if (carrito[elemento.nombre].cantidad === 0) {
                    delete carrito[elemento.nombre]
                }
            }
        })
    }
    pintarTabla(carrito)
    pintarFooter()
}