<template>
  <div class="about-container">
    <router-link class="backToStatsLink" to="/">tillbaka till statistiken</router-link>
    <h3>
      Skiljer det sig i hur olika tidningar väljer att skriva om
      <span
        id="theSubject"
        class="text-info"
      >olika partier</span>?
    </h3>
    <p>
      MediaSpin är ett hobbyprojekt som jag sporadiskt knackar på under lediga halvtimmar. Idén är att, mha sentimentanalys, följa hur media
      väljer att rapportera om olika ämnen över tid. Tjänsten fungerar genom att dagligen leta igenom nättidningar efter vissa nyckelord
      samt utföra sentimentanalys på identifierade meningar. Resultatet av denna analys sammanställs och visas på denna enkla webbplats.
      Idag så täcker tjänsten in följande svenska tidningar:
    </p>
    <ul>
      <li>www.svt.se (Oberoende)</li>
      <li>www.aftonbladet.se (Oberoende socialdemokratisk)</li>
      <li>www.svd.se (Obunden moderat/Obunden konservativ)</li>
      <li>www.dn.se (Oberoende liberal)</li>
      <li>www.gp.se (Oberoende liberal)</li>
      <li>www.unt.se (Oberoende liberal)</li>
      <li>www.expressen.se (Obunden liberal)</li>
      <li>www.sydsvenskan.se (Oberoende liberal)</li>
      <li>www.dalademokraten.se (Oberoende socialdemokratisk)</li>
      <li>www.nsd.se (Oberoende socialdemokratisk)</li>
    </ul>

    <p>
      Prova byta nyckelord eller månad samt filtrera på positivt eller negativt. Du kan, förrutom diagramet, även välja att se
      det underliggande datat i löpande text.
    </p>

    <p>
      För den som är intresserad så kan man läsa mer om hur denna lösning fungerar rent tekniskt på
      <a
        class="blue-link"
        href="https://github.com/hellgrenj/MediaSpin"
      >projektets Readme.</a>
    </p>
  </div>
</template>

<script lang="ts">
import axios from "axios";
import { startRollingSubject } from "../switch-subject-on-interval";
export default {
  data() {
    return {};
  },
  async mounted() {
    await axios
      .get("/api/index/allkeywords")
      .then(res => {
        let keywords = res.data;
        startRollingSubject({keywords});
      })
      .catch(err => {
        console.log(err);
      });
  }
};
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.backToStatsLink {
  display:block;
  color:#ccc;
  padding-bottom:10px;
  }
  .text-info {color:#3e95cd!important;}
</style>
