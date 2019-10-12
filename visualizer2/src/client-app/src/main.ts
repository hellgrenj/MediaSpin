import Vue from 'vue';
import App from './App.vue';
import router from './router/index';
import 'bootstrap'; 
import 'bootstrap/dist/css/bootstrap.min.css';
import '../public/css/site.css'

Vue.config.productionTip = false;
Vue.use(require('vue-moment'));

new Vue({
  router,
  render: (h) => h(App),
}).$mount('#app');