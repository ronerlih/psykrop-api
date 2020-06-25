import React from "react";
import styles from "./style.module.css";

export default ({children}) =>
	<h5 className={styles.title}>
       {children}
    </h5>